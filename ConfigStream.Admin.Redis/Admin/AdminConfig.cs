﻿using ConfigStream.Admin.Redis.Entities;
using ConfigStream.Admin.Redis.Models;
using ConfigStream.Redis;
using StackExchange.Redis;
using System.Text.Json;

namespace ConfigStream.Admin.Redis
{
    public class AdminConfig : IAdminConfig
    {
        private readonly IDatabase _database;
        public AdminConfig(IDatabase database)
        {
            _database = database;
        }
        public Task CreateOrUpdateConfigAsync(Config config)
        {
            string groupName = config.GroupName;
            string configName = config.ConfigName;

            return StoreConfigInTransactionAsync(groupName, configName, config);
        }        
        private async Task RemoveValueAsync(string groupName, string configName, string environmentName, string? targetName = null)
        {
            RedisKey redisKey = RedisKeys.EnvironmentTargetValue(environmentName, groupName, configName, targetName);
            await _database.KeyDeleteAsync(redisKey);
        }

        private async Task SetValueAsync(string value, string groupName, string configName, string environmentName, string? targetName = null)
        {
            await CheckAllowedValues(groupName, configName, value);
            RedisKey redisKey = RedisKeys.EnvironmentTargetValue(environmentName, groupName, configName, targetName);
            await _database.StringSetAsync(redisKey, value);
        }
        public Task SetConfigValueAsync(SubmitConfigValue[] configValues)
        {
            List<Task> tasks = new List<Task>();
            foreach (SubmitConfigValue configValue in configValues)
            {
                Task task = string.IsNullOrWhiteSpace(configValue.Value)
                    ? RemoveValueAsync(
                        groupName: configValue.GroupName,
                        configName: configValue.ConfigName,
                        environmentName: 
                        configValue.EnvironmentName,
                        targetName: configValue.TargetName)
                    : SetValueAsync(
                        value: configValue.Value, 
                        groupName: configValue.GroupName,
                        configName: configValue.ConfigName,
                        environmentName: configValue.EnvironmentName,
                        targetName: configValue.TargetName);
                
                tasks.Add(task);
            }
            return Task.WhenAll(tasks);
        }

        public async Task<Config?> GetConfigAsync(string groupName, string configName)
        {
            var key = $"ConfigSteam:Configs:{groupName}:{configName}";
            RedisValue redisConfig = await _database.StringGetAsync(key);
            if (redisConfig.IsNullOrEmpty)
            {
                return null;
            }
            Config? config = JsonSerializer.Deserialize<Config>(redisConfig!);
            return config;
        }

        public async Task<Config[]> GetConfigsAsync()
        {
            var prefix = "ConfigSteam:Configs:";
            RedisKey[] keys = await ScanKeysAsync($"{prefix}*");
            RedisValue[] redisConfigs = await _database.StringGetAsync(keys);
            if (redisConfigs is null)
            {
                return Array.Empty<Config>();
            }

            Config[] configs = redisConfigs
                .Where(redisConfig => !redisConfig.IsNullOrEmpty)
                .Select(redisConfig => JsonSerializer.Deserialize<Config>(redisConfig!))
                .OfType<Config>()
                .ToArray();

            return configs;
        }
        
        public async Task DeleteConfigAsync(string groupName, string configName)
        {
            string redisKey = RedisKeys.Config(groupName, configName);
            RedisValue redisValue = await _database.StringGetAsync(redisKey);
            if (redisValue.IsNullOrEmpty)
            {
                return;
            }
            
            Config config = JsonSerializer.Deserialize<Config>(redisValue!)!;
            await DeleteConfigInTransactionAsync(config);
        }

        public async Task<ConfigValue[]> GetValuesAsync()
        {
            var prefix = "ConfigSteam:Values:Environments:";
            RedisKey[] keys = await ScanKeysAsync($"{prefix}*");
            RedisValue[] redisValues = await _database.StringGetAsync(keys);
            if (redisValues is null)
            {
                return Array.Empty<ConfigValue>();
            }
            
            ConfigValue[] configValues = keys
                .Select(x => x.ToString().Substring(prefix.Length).Split(":"))
                .Select((x, i) => new ConfigValue
                {
                    EnvironmentName = x[0],
                    GroupName = x[1],
                    ConfigName = x[2],
                    TargetName = x.Length >= 4 ? x[3] : null,
                    Value = redisValues[i]!
                }).ToArray();

            return configValues;
        }

        public async Task<ConfigValue?> GetValueAsync(string groupName, string configName, string environment, string? target = null)
        {
            var key = $"ConfigSteam:Values:Environments:{environment}:{groupName}:{configName}";
            if (!string.IsNullOrEmpty(target)) 
            {
                key += $":{target}";
            }
            RedisValue redisValues = await _database.StringGetAsync(key);
            if (redisValues.IsNullOrEmpty)
            {
                return null;
            }

            return new ConfigValue
            {
                EnvironmentName = environment,
                GroupName = groupName,
                ConfigName = configName,
                TargetName = target,
                Value = redisValues!
            };
        }

        #region Private
        private async Task<RedisKey[]> ScanKeysAsync(string mask)
        {
            List<RedisKey> keys = new ();            
            int cursor = 0;
            do
            {
                RedisResult result = await _database.ExecuteAsync("SCAN", cursor, "MATCH", mask);
                var innerResult = (RedisResult[]) result!;
                cursor = int.Parse((string) innerResult[0]!);
                keys.AddRange((RedisKey[]) innerResult[1]!);
            } while (cursor != 0);

            return keys.ToArray();
        }
        private Task DeleteConfigInTransactionAsync(Config config)
        {
            ITransaction transaction = _database.CreateTransaction();
            transaction.KeyDeleteAsync(RedisKeys.Config(config.GroupName, config.ConfigName));
            return transaction.ExecuteAsync();
        }
        private Task StoreConfigInTransactionAsync(string groupName, string configName, Config config)
        {
            string json = JsonSerializer.Serialize(config);

            ITransaction transaction = _database.CreateTransaction();
            transaction.StringSetAsync(RedisKeys.Config(groupName, configName), new RedisValue(json));
            return transaction.ExecuteAsync();
        }
        private async Task CheckAllowedValues(string groupName, string configName, string value)
        {
            RedisValue redisValue = await _database.StringGetAsync(RedisKeys.Config(groupName, configName));
            if (redisValue.IsNullOrEmpty)
            {
                Console.WriteLine($"The config {groupName}:{configName} doesn't exist");
                return;
            }

            Config? config = JsonSerializer.Deserialize<Config>(redisValue!);
            if (config?.AllowedValues != null && config.AllowedValues.Length > 0 && !config.AllowedValues.Contains(value))
            {
                throw new Exception($"The value {value} for config {groupName}:{configName} is not allowed");
            }
        }
        #endregion
    }
}
