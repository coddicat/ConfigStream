using ConfigStream.Admin.Redis.Entities;
using ConfigStream.Admin.Redis.Models;

namespace ConfigStream.Admin.Redis
{
    public interface IAdminConfig
    {
        Task CreateOrUpdateConfigAsync(Config config);        
        Task DeleteConfigAsync(string groupName, string configName);
        Task SetConfigValueAsync(SubmitConfigValue[] configValue);
        Task<ConfigValue[]> GetValuesAsync();
        Task<ConfigValue?> GetValueAsync(string groupName, string configName, string environment, string? target = null);
        Task<Config[]> GetConfigsAsync();
        Task<Config?> GetConfigAsync(string groupName, string configName);
    }
}
