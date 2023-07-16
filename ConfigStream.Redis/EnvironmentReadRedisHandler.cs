using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Handler;
using StackExchange.Redis;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConfigStream.Redis
{
    public class EnvironmentReadRedisHandler : IConfigStreamHandler
    {
        private readonly IDatabase _database;
        public EnvironmentReadRedisHandler(IDatabase database)
        {
            _database = database;
        }
        public async Task HandleAsync(IHandlerContext context, HandlerDelegate next)
        {
            RedisKey envRedisKey = RedisKeys.EnvironmentValue(context.EnvironmentName, context.GroupName, context.ConfigName, context.TargetName);
            if (!context.Value.IsSuccess)
            {
                await TryReadFromRedisAsync(envRedisKey, context);
            }

            await next(context);
        }

        private async Task TryReadFromRedisAsync(RedisKey scopeRedisKey, IHandlerContext context)
        {
            RedisValue value = await _database.StringGetAsync(scopeRedisKey);

            if (!value.HasValue)
            {
                await TryReadDefaultFromRedisAsync(context);
                return;
            }

            context.Value = new ConfigStreamValue
            {
                Data = value,
                IsSuccess = true
            };
        }

        private async Task TryReadDefaultFromRedisAsync(IHandlerContext context)
        {
            var configKey = RedisKeys.Config(context.GroupName, context.ConfigName);
            var configRedisValue = await _database.StringGetAsync(configKey);
            if (!configRedisValue.HasValue)
            {
                return;
            }
            
            var config = JsonSerializer.Deserialize<Config>(configRedisValue);
            if (config?.DefaultValue is null)
            {
                return;
            }

            context.Value = new ConfigStreamValue
            {
                IsSuccess = true,
                IsDefault = true,
                Data = config.DefaultValue,
            };
        }
    }

}
