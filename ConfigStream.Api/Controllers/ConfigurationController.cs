using ConfigStream.Abstractions;
using ConfigStream.Admin.Redis;
using ConfigStream.Admin.Redis.Entities;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{    
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IConfigStreamService _configStreamService;
        private readonly IAdminConfig _adminConfig;
        private readonly IConfigStreamScope _configScope;

        public ConfigurationController(ILogger<ConfigurationController> logger, IConfigStreamService configStreamService, IAdminConfig adminConfig)
        {
            _logger = logger;
            _configStreamService = configStreamService;
            _adminConfig = adminConfig;
            _configScope = _configStreamService.CreateScope();
        }

        [HttpPost("Config")]
        public Task CreateConfigAsync(Config config)
        {
            return _adminConfig.CreateOrUpdateConfigAsync(config);
        }

        //[HttpGet("Group")]
        //public Task<ConfigGroup[]> GetGroupsAsync()
        //{
        //    return _adminConfig.GetGroupsAsync();
        //}

        //[HttpGet("Config")]
        //public Task<Config[]> GetConfigAsync(string search = null)
        //{
        //    return _adminConfig.GetConfigsAsync(search);
        //}

        [HttpPost("Set")]
        public async Task SetGetAsync()
        {
            //var admin = new AdminConfig(_db);
            //var service = new ConfigStreamService(_db, "Production");
            //await admin.SetEnvironmentValueAsync("Production", "Infra", "Feature1", "on");
            //await admin.SetEnvironmentValueAsync("Production", "Infra", "Feature1", "on", "Client1");

            //var scope = service.CreateScope();


            var res = await _configStreamService.ReadAsync("Infra", "Feature2");


            var result1 = await _configScope.ReadAsync("Infra", "Feature2");
            await _configScope.WriteAsync("hello world", "Infra", "Feature2");
            var result2 = await _configScope.ReadAsync("Infra", "Feature2");
            var result3 = await _configScope.ReadAsync("Infra", "Feature2", "Client1");
        }
        //[HttpPost("Environment")]
        //public async Task AddEnvironmentAsync(Models.Environment environment)
        //{
        //    string value = JsonSerializer.Serialize(environment);
        //    await _db.ListRightPushAsync("environments", value);
        //}
        //[HttpPost("Group")]
        //public async Task AddGroupAsync(Group group)
        //{
        //    string value = JsonSerializer.Serialize(group);
        //    await _db.ListRightPushAsync("groups", value);
        //}


        //[HttpPost("Configuration")]
        //public async Task AddConfigurationAsync(Configuration configuration)
        //{
        //    string value = JsonSerializer.Serialize(configuration);
        //    await _db.ListRightPushAsync($"configurations:{configuration.Group}", value);
        //}

        //[HttpPost("ConfigurationValue")]
        //public async Task SetConfigurationAsync(ConfigurationValue configurationValue)
        //{
        //    var configurationValues = await _db.ListRangeAsync($"configurations:{configurationValue.Group}");
        //    var configurations = configurationValues
        //        .Select(x => JsonSerializer.Deserialize<Configuration>(x))
        //        .ToList();
        //    var configuration = configurations
        //        .Where(x => x.Group == configurationValue.Group)
        //        .FirstOrDefault(x => x.Name == configurationValue.Name);

        //    if (configuration is null)
        //    {
        //        throw new ArgumentNullException(nameof(configuration));
        //    }

        //    if(!configuration.AllowedValues.Contains(configurationValue.Value))
        //    {
        //        throw new ArgumentOutOfRangeException(nameof(configurationValue.Value));
        //    }

        //    string value = JsonSerializer.Serialize(configurationValue);
        //    await _db.StringSetAsync($"values:{configurationValue.Group}:{configurationValue.Name}:{configurationValue.Environment}", configurationValue.Value);
        //}
    }


}