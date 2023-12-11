using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Handler;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConfigStream.Redis
{
    public class ScopeWriteRedisHandler : IConfigStreamHandler
    {
        private readonly IDatabase _database;
        public ScopeWriteRedisHandler(IDatabase database)
        {
            _database = database;
        }
        public async Task HandleAsync(IHandlerContext context, HandlerDelegate next)
        {           
            if (context.Value.IsSuccess)
            {
                await ValidateAllowedValuesAsync(context);
                await WriteRedisAsync(context);
            }

            await next(context);
        }
        private async Task WriteRedisAsync(IHandlerContext context)
        {
            RedisKey scopeRedisKey = RedisKeys.ScopeValue(context.ScopeId, context.GroupName, context.ConfigName, context.TargetName);
            RedisValue value = context.Value.Data;
            await _database.StringSetAsync(scopeRedisKey, value, TimeSpan.FromSeconds(100));
        }
        private async Task ValidateAllowedValuesAsync(IHandlerContext context)
        {
            var value = context.Value.Data;
            RedisValue redisValue = await _database.StringGetAsync(RedisKeys.Config(context.GroupName, context.ConfigName));
            Config config = JsonSerializer.Deserialize<Config>(redisValue);
            if (config?.AllowedValues != null && !config.AllowedValues.Contains(value))
            {
                throw new Exception($"The value {value} for config {context.GroupName}:{context.ConfigName} is not allowed");
            }
        }
    }
}
