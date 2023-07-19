using ConfigStream.Admin.Redis.Entities;
using ConfigStream.Admin.Redis.Models;
using ConfigStream.Redis;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace ConfigStream.Admin.Redis
{
    public interface IAdminConfig
    {
        Task CreateOrUpdateGroupAsync(ConfigGroup group);
        Task CreateOrUpdateTargetAsync(ConfigTarget target);
        Task CreateOrUpdateConfigAsync(Config config);
        Task CreateOrUpdateEnvironmentAsync(ConfigEnvironment environment);
        
        Task DeleteConfigAsync(string groupName, string configName);
        Task DeleteConfigGroupAsync(string groupName);
        Task DeleteEnvironmentAsync(string environmentName);
        Task DeleteTargetAsync(string targetName);

        Task SetConfigValueAsync(SubmitConfigValue configValue);

        Task<ConfigValue[]> GetValuesAsync(string[] environments, string search = null);
        Task<ConfigGroup[]> GetGroupsAsync(string search = null);
        Task<Config[]> GetConfigsAsync(string search = null);
        Task<ConfigTarget[]> GetTargetsAsync(string search = null);
        Task<ConfigEnvironment[]> GetEnvironmentsAsync(string search = null);        
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
        public Task CreateOrUpdateEnvironmentAsync(ConfigEnvironment environment)
        {
            string environmentName = environment.Name;
            string json = JsonSerializer.Serialize(environment);

            ITransaction transaction = _database.CreateTransaction();
            transaction.SetAddAsync(RedisKeys.EnvironmentKeys, environmentName);
            transaction.StringSetAsync(RedisKeys.Environment(environmentName), new RedisValue(json));
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
        
        private async Task SetEnvironmentValueAsync(string groupName, string configName, string environmentName, string value)
        {
            await CheckKeysExist(groupName, configName, environmentName/*, targetName*/);
            await CheckAllowedValues(groupName, configName, value);
            RedisKey redisKey = RedisKeys.EnvironmentValue(environmentName, groupName, configName/*, targetName*/);
            await _database.StringSetAsync(redisKey, value);
        }
        public Task SetConfigValueAsync(SubmitConfigValue configValue)
        {
            List<Task> tasks = new List<Task>();
            foreach (KeyValuePair<string, string> environmentValue in configValue.EnvironmentValues)
            {
                tasks.Add(SetEnvironmentValueAsync(configValue.GroupName, configValue.ConfigName, environmentValue.Key, environmentValue.Value));
            }
            return Task.WhenAll(tasks);
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
        public async Task<ConfigTarget[]> GetTargetsAsync(string search = null)
        {
            RedisValue[] targetNames = await _database.SetMembersAsync(RedisKeys.TargetKeys);
            RedisKey[] redisKeys = targetNames
                .Where(targetName => string.IsNullOrWhiteSpace(search) || ((string)targetName).Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(targetName => RedisKeys.Target(targetName))
                .Select(x => new RedisKey(x))
                .ToArray();

            RedisValue[] redisValues = await _database.StringGetAsync(redisKeys);

            ConfigTarget[] targets = redisValues
                .Select(x => JsonSerializer.Deserialize<ConfigTarget>(x))
                .ToArray();

            return targets;
        }
        public async Task<ConfigEnvironment[]> GetEnvironmentsAsync(string search = null)
        {
            RedisValue[] environmentNames = await _database.SetMembersAsync(RedisKeys.EnvironmentKeys);
            RedisKey[] redisKeys = environmentNames
                .Where(environmentName => string.IsNullOrWhiteSpace(search) || ((string)environmentName).Contains(search, StringComparison.InvariantCultureIgnoreCase))
                .Select(environmentName => RedisKeys.Environment(environmentName))
                .Select(x => new RedisKey(x))
                .ToArray();

            RedisValue[] redisValues = await _database.StringGetAsync(redisKeys);

            ConfigEnvironment[] environments = redisValues
                .Select(x => JsonSerializer.Deserialize<ConfigEnvironment>(x))
                .ToArray();

            return environments;
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
        public async Task DeleteEnvironmentAsync(string environmentName)
        {
            await DeleteEnvironmentInTransactionAsync(environmentName);
        }
        public async Task DeleteTargetAsync(string targetName)
        {
            await DeleteTargetInTransactionAsync(targetName);
        }

        public async Task<ConfigValue[]> GetValuesAsync(string[] environments, string search = null)
        {
            RedisValue[] groupNames = await _database.SetMembersAsync(RedisKeys.GroupKeys);            
            Dictionary<RedisKey, (Config config, string env)> configByKey = new Dictionary<RedisKey, (Config config, string env)>();
            foreach (string groupName in groupNames)
            {                
                Config[] configs = await GetConfigsByGroupAsync(groupName);
                Dictionary<RedisKey, (Config config, string env)> dic =
                    configs.SelectMany(config => environments.Select(environmentName => new
                    {
                        Environment = environmentName,
                        ConfigName = config.Name,
                        Config = config
                    })).ToDictionary(
                    x => new RedisKey(RedisKeys.EnvironmentValue(x.Environment, groupName, x.ConfigName)),
                    x => (x.Config, x.Environment));

                configByKey = configByKey.Union(dic).ToDictionary(x => x.Key, x => x.Value);
            }

            RedisKey[] redisKeys = configByKey.Keys.ToArray();
            RedisValue[] redisValues = await _database.StringGetAsync(redisKeys);

            var result = redisKeys.Select((key, i) => new
                {
                    config = configByKey[key],
                    value = redisValues[i],
                }).GroupBy(x => x.config.config)
                .Select(x => new ConfigValue
                {
                    ConfigName = x.Key.Name,
                    GroupName = x.Key.GroupName,
                    DefaultValue = x.Key.DefaultValue,
                    AllowedValues = x.Key.AllowedValues,
                    EnvironmentValues = x.ToDictionary(y => y.config.env, y => (string)y.value)
                })
                .ToArray();

            return result;
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
        private Task DeleteEnvironmentInTransactionAsync(string environmentName)
        {
            ITransaction transaction = _database.CreateTransaction();
            transaction.SetRemoveAsync(RedisKeys.EnvironmentKeys, environmentName);
            transaction.KeyDeleteAsync(RedisKeys.Environment(environmentName));
            return transaction.ExecuteAsync();
        }
        private Task DeleteTargetInTransactionAsync(string targetName)
        {
            ITransaction transaction = _database.CreateTransaction();
            transaction.SetRemoveAsync(RedisKeys.TargetKeys, targetName);
            transaction.KeyDeleteAsync(RedisKeys.Target(targetName));
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
        private async Task CheckKeysExist(string groupName, string configName, string environmentName, string targetName = null)
        {
            bool configExists = await _database.SetContainsAsync(RedisKeys.ConfigKeys(groupName), configName);
            if (!configExists)
            {
                throw new Exception($"Config {groupName}:{configName} doesn't exist");
            }

            bool environmentExists = await _database.SetContainsAsync(RedisKeys.EnvironmentKeys, environmentName);
            if (!environmentExists)
            {
                throw new Exception($"Environment {environmentName} doesn't exist");
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
