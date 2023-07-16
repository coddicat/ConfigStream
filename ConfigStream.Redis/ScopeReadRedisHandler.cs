using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Handler;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ConfigStream.Redis
{
    public class ScopeReadRedisHandler : IConfigStreamHandler
    {
        private readonly IDatabase _database;
        public ScopeReadRedisHandler(IDatabase database)
        {
            _database = database;
        }
        public async Task HandleAsync(IHandlerContext context, HandlerDelegate next)
        {
            RedisKey scopeRedisKey = RedisKeys.ScopeValue(context.ScopeId, context.GroupName, context.ConfigName, context.TargetName);
            bool fromRedis = false;
            if (!context.Value.IsSuccess)
            {
                fromRedis = await TryReadFromRedisAsync(scopeRedisKey, context);
            }

            await next(context);

            if (context.Value.IsSuccess && !fromRedis)
            {
                bool keyExists = await _database.KeyExistsAsync(scopeRedisKey);
                if (keyExists)
                {
                    return;
                }

                await _database.StringSetAsync(scopeRedisKey, context.Value.Data, TimeSpan.FromSeconds(100));
            }
        }
        private async Task<bool> TryReadFromRedisAsync(RedisKey scopeRedisKey, IHandlerContext context)
        {
            RedisValue value = await _database.StringGetAsync(scopeRedisKey);

            if (!value.HasValue)
            {
                return false;
            }

            context.Value = new ConfigStreamValue
            {
                Data = value,
                IsSuccess = true
            };

            return true;
        }
    }
}
