using ConfigStream.Admin.Redis.Entities;
using ConfigStream.Admin.Redis.Models;
using ConfigStream.Redis;
using StackExchange.Redis;
using System.Text.Json;

namespace ConfigStream.Admin.Redis
{
    public interface IAdminConfig
    {
        Task CreateOrUpdateGroupAsync(ConfigGroup group);
        Task CreateOrUpdateTargetAsync(ConfigTarget target);
        Task CreateOrUpdateConfigAsync(Config config);
        Task DeleteConfigAsync(string groupName, string configName);
        Task DeleteConfigGroupAsync(string groupName);

        Task SetEnvironmentValueAsync(string environmentName, string groupName, string configName, string value, string targetName = null);

        Task<ConfigValue[]> GetValuesAsync(string environment, string search = null);
        Task<ConfigGroup[]> GetGroupsAsync(string search = null);
        Task<Config[]> GetConfigsAsync(string search = null);
        Task<ConfigTarget[]> GetTargetsAsync(string search = null);
    }

    public class AdminConfig : IAdminConfig
    {
        private readonly IDatabase _database;

        public AdminConfig(IDatabase database)
        {
            _database = database;
        }

        #region Public
        public Task CreateOrUpdateGroupAsync(ConfigGroup group)
        {
            string groupName = group.Name;
            string json = JsonSerializer.Serialize(group);

            ITransaction transaction = _database.CreateTransaction();
            transaction.SetAddAsync(RedisKeys.GroupKeys, groupName);
            transaction.StringSetAsync(RedisKeys.Group(groupName), new RedisValue(json));
            return transaction.ExecuteAsync();
        }
        public Task CreateOrUpdateTargetAsync(ConfigTarget target)
        {
            string targetName = target.Name;
            string json = JsonSerializer.Serialize(target);

            ITransaction transaction = _database.CreateTransaction();
            transaction.SetAddAsync(RedisKeys.TargetKeys, targetName);
            transaction.StringSetAsync(RedisKeys.Target(targetName), new RedisValue(json));
            return transaction.ExecuteAsync();
        }
        public async Task CreateOrUpdateConfigAsync(Config config)
        {
            string groupName = config.GroupName;
            string configName = config.Name;

            var groupExists = await _database.SetContainsAsync(RedisKeys.GroupKeys, groupName);
            if (!groupExists)
            {
                throw new Exception($"Group {groupName} doesn't exist");
            }            
            await StoreConfigInTransactionAsync(groupName, configName, config);
        }
        public async Task SetEnvironmentValueAsync(string environmentName, string groupName, string configName, string value, string targetName = null)
        {
            await CheckKeysExist(groupName, configName, targetName);
            await CheckAllowedValues(groupName, configName, value);
            RedisKey redisKey = RedisKeys.EnvironmentValue(environmentName, groupName, configName, targetName);
            await _database.StringSetAsync(redisKey, value);
        }
        public async Task<ConfigGroup[]> GetGroupsAsync(string search = null)
        {
            RedisValue[] groupNames = await _database.SetMembersAsync(RedisKeys.GroupKeys);
            RedisKey[] redisKeys = groupNames
                .Where(groupName => string.IsNullOrWhiteSpace(search) || ((string)groupName).Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(groupName => RedisKeys.Group(groupName))
                .Select(x => new RedisKey(x))
                .ToArray();

            RedisValue[] redisValues = await _database.StringGetAsync(redisKeys);

            ConfigGroup[] groups = redisValues
                .Select(x => JsonSerializer.Deserialize<ConfigGroup>(x))
                .ToArray();

            return groups;
        }
        public Task<ConfigTarget[]> GetTargetsAsync(string search = null)
        {
            throw new NotImplementedException();
        }
        public async Task<Config[]> GetConfigsAsync(string search = null)
        {
            RedisValue[] groupNames = await _database.SetMembersAsync(RedisKeys.GroupKeys);
            List<Config> allConfigs = new List<Config>();
            foreach (string groupName in groupNames)
            {
                Config[] configs = await GetConfigsByGroupAsync(groupName, search);
                allConfigs.AddRange(configs);
            }

            return allConfigs.ToArray();
        }
        public async Task DeleteConfigAsync(string groupName, string configName)
        {
            string redisKey = RedisKeys.Config(groupName, configName);
            RedisValue redisValue = await _database.StringGetAsync(redisKey);
            Config config = JsonSerializer.Deserialize<Config>(redisValue);

            await DeleteConfigInTransactionAsync(config);
        }
        public async Task DeleteConfigGroupAsync(string groupName)
        {
            await DeleteConfigGroupInTransactionAsync(groupName);
        }
        public async Task<ConfigValue[]> GetValuesAsync(string environment, string search = null)
        {
            RedisValue[] groupNames = await _database.SetMembersAsync(RedisKeys.GroupKeys);            
            Dictionary<RedisKey, Config> configByKey = new Dictionary<RedisKey, Config>();
            foreach (string groupName in groupNames)
            {                
                Config[] configs = await GetConfigsByGroupAsync(groupName);
                var dic = configs.ToDictionary(
                    config => new RedisKey(RedisKeys.EnvironmentValue(environment, groupName, config.Name)),
                    config => config);

                configByKey = configByKey.Union(dic).ToDictionary(x => x.Key, x => x.Value);
            }

            RedisKey[] redisKeys = configByKey.Keys.ToArray();
            RedisValue[] redisValues = await _database.StringGetAsync(redisKeys);
            
            List<ConfigValue> result = new List<ConfigValue>();
            for (int i = 0; i < redisKeys.Length; i++)
            {
                RedisKey key = redisKeys[i];
                result.Add(new ConfigValue
                {
                    GroupName = configByKey[key].GroupName,
                    ConfigName = configByKey[key].Name,
                    DefaultValue = configByKey[key].DefaultValue,
                    Value = redisValues[i],
                });
            }

            return result.ToArray();
        }
        #endregion

        #region Private
        private Task DeleteConfigInTransactionAsync(Config config)
        {
            ITransaction transaction = _database.CreateTransaction();
            transaction.SetRemoveAsync(RedisKeys.ConfigKeys(config.GroupName), config.Name);
            transaction.KeyDeleteAsync(RedisKeys.Config(config.GroupName, config.Name));
            return transaction.ExecuteAsync();
        }
        private Task DeleteConfigGroupInTransactionAsync(string groupName)
        {
            ITransaction transaction = _database.CreateTransaction();
            transaction.SetRemoveAsync(RedisKeys.GroupKeys, groupName);
            transaction.KeyDeleteAsync(RedisKeys.Group(groupName));
            return transaction.ExecuteAsync();
        }
        private Task StoreConfigInTransactionAsync(string groupName, string configName, Config config)
        {
            string json = JsonSerializer.Serialize(config);

            ITransaction transaction = _database.CreateTransaction();
            transaction.SetAddAsync(RedisKeys.ConfigKeys(groupName), configName);
            transaction.StringSetAsync(RedisKeys.Config(groupName, configName), new RedisValue(json));
            return transaction.ExecuteAsync();
        }
        private async Task CheckKeysExist(string groupName, string configName, string targetName)
        {
            var configExists = await _database.SetContainsAsync(RedisKeys.ConfigKeys(groupName), configName);
            if (!configExists)
            {
                throw new Exception($"Config {groupName}:{configName} doesn't exist");
            }
            if (targetName is not null)
            {
                var targetExists = await _database.SetContainsAsync(RedisKeys.TargetKeys, targetName);
                if (!targetExists)
                {
                    throw new Exception($"Target {targetName} doesn't exist");
                }
            }
        }
        private async Task CheckAllowedValues(string groupName, string configName, string value)
        {
            var redisValue = await _database.StringGetAsync(RedisKeys.Config(groupName, configName));
            var config = JsonSerializer.Deserialize<Config>(redisValue);
            if (config.AllowedValues != null && config.AllowedValues.Length > 0 && !config.AllowedValues.Contains(value))
            {
                throw new Exception($"The value {value} for config {groupName}:{configName} is not allowed");
            }
        }
        private async Task<Config[]> GetConfigsByGroupAsync(string groupName, string search = null)
        {
            RedisValue[] configNames = await _database.SetMembersAsync(RedisKeys.ConfigKeys(groupName));
            RedisKey[] redisKeys = configNames
                .Where(configName => string.IsNullOrWhiteSpace(search) || ((string)configName).Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(configName => RedisKeys.Config(groupName, configName))
                .Select(x => new RedisKey(x))
                .ToArray();

            RedisValue[] redisValues = await _database.StringGetAsync(redisKeys);

            Config[] groups = redisValues
                .Select(x => JsonSerializer.Deserialize<Config>(x))
                .ToArray();

            return groups;
        }
        #endregion
    }
}
