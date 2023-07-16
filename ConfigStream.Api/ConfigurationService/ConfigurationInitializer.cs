using StackExchange.Redis;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace WebApplication1.ConfigurationService
{
    public class ConfigurationInitializer
    {
        private readonly IDatabase _db;
        public ConfigurationInitializer([NotNull] IDatabase db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public Task InitGroupKeysAsync<TGroup>() where TGroup : struct, Enum
        {
            var groups = Enum.GetNames<TGroup>().Select(x => new RedisValue(x)).ToArray();
            return _db.SetAddAsync("groups:keys", groups);
        }

        private async Task InitConfigurationKeysAsync<TConfiguration>(string groupName) where TConfiguration : struct, Enum
        {
            //var groupName = group.ToString();
            //await InitGroupKeysAsync<TGroup>();
            await _db.SetAddAsync("groups:keys", new RedisValue(groupName));
            var configurations = Enum.GetNames<TConfiguration>().Select(x => new RedisValue(x)).ToArray();
            await _db.SetAddAsync($"configurations:{groupName}:keys", configurations);
        }

        private Configuration GetConfiguration<TConfiguration>(TConfiguration configurationValue)
        {
            FieldInfo fieldInfo = typeof(TConfiguration).GetField(configurationValue.ToString());
            Configuration configuration = new Configuration
            {
                AllowedValues = fieldInfo.GetCustomAttribute<AllowedValuesAttribute>()?.Values,
                DefaultValue = fieldInfo.GetCustomAttribute<DefaultValueAttribute>()?.Value?.ToString(),
                Description = fieldInfo.GetCustomAttribute<DescriptionAttribute>()?.Description,
            };
            return configuration;
        }

        public async Task InitConfigurationAsync<TConfiguration>() where TConfiguration : struct, Enum
        {
            ConfigurationGroupAttribute groupAttr = typeof(TConfiguration).GetCustomAttribute<ConfigurationGroupAttribute>();
            var groupName = groupAttr?.GroupName ?? "Default";
            //await InitConfigurationKeysAsync<TConfiguration>(group);
            Dictionary<RedisKey, RedisValue> configurations = Enum.GetValues<TConfiguration>().ToDictionary(
                x => new RedisKey($"configurations:{groupName}:values:{Enum.GetName(x)}"), 
                x => new RedisValue(JsonSerializer.Serialize(GetConfiguration(x))));
            await _db.StringSetAsync(configurations.ToArray());
        }

    }

    public class Configuration
    {
        public string Description { get; set; }
        public string[] AllowedValues { get; set; }
        public string DefaultValue { get; set; }
    }

}
