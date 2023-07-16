namespace ConfigStream.Admin.Redis
{
    //public interface IConfigScope
    //{
    //    string ScopeId { get; }
    //    Task<string> GetValueAsync(string groupName, string configName, string targetName = null);
    //}
    //public class ConfigScope: IConfigScope
    //{
    //    private readonly IDatabase _database;
    //    private readonly string _environment;
    //    private readonly ConcurrentDictionary<string, string> _localCache = new ConcurrentDictionary<string, string>();
    //    internal ConfigScope(IDatabase database, string scopeId, string environment)
    //    {
    //        _database = database;
    //        _environment = environment;
    //        ScopeId = scopeId;            
    //    }

    //    public string ScopeId { get; private set; }

    //    public async Task<string> GetValueAsync(string groupName, string configName, string targetName = null)
    //    {
    //        RedisKey scopeRedisKey = ConfigEntityKeys.ScopeValue(ScopeId, groupName, configName, targetName);
    //        string cache = _localCache.GetValueOrDefault(scopeRedisKey);
    //        if (cache != null) 
    //        {
    //            return cache;
    //        }

    //        RedisValue value = await _database.StringGetAsync(scopeRedisKey);
    //        if (!value.HasValue)
    //        {
    //            RedisKey envRedisKey = ConfigEntityKeys.EnvironmentValue(_environment, groupName, configName, targetName);
    //            value = await _database.StringGetAsync(envRedisKey);
    //        }

    //        if (!value.HasValue)
    //        {
    //            var configKey = ConfigEntityKeys.Config(groupName, configName);
    //            var configRedisValue = await _database.StringGetAsync(configKey);
    //            if (!configRedisValue.HasValue)
    //            {
    //                return null;
    //            }
    //            var config = JsonSerializer.Deserialize<Config>(configRedisValue);
    //            if (config.DefaultValue is null)
    //            {
    //                return null;
    //            }
    //            value = new RedisValue(config.DefaultValue);
    //        }

    //        await _database.StringSetAsync(scopeRedisKey, value, TimeSpan.FromSeconds(100));
    //        var result = value.ToString();
    //        _localCache[scopeRedisKey] = result;
    //        return result;
    //    }
    //}
}
