using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Handler;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConfigStream.Pipeline
{
    public class ScopeReadLocalHandler : IConfigStreamHandler
    {
        private readonly ConcurrentDictionary<string, string> _localCache;

        public ScopeReadLocalHandler(ScopeLocalCache cache)
        {
            _localCache = cache.Cache;
        }
        public async Task HandleAsync(IHandlerContext context, HandlerDelegate next)
        {
            string key = ReadLocalCacheKey(context);

            if (!context.Value.IsSuccess)
            {
                TryGetValueFromLocalCache(key, context);
            }

            await next(context);

            if (context.Value.IsSuccess)
            {
                _localCache[key] = context.Value.Data;
            }
        }

        private static string ReadLocalCacheKey(IHandlerContext context)
        {
            string key = $"{context.ScopeId}:{context.GroupName}:{context.ConfigName}";
            if (!string.IsNullOrWhiteSpace(context.TargetName))
            {
                key += $":{context.TargetName}";
            }

            return key;
        }

        private void TryGetValueFromLocalCache(string key, IHandlerContext context)
        {            
            if (!_localCache.TryGetValue(key, out string value))
            {
                return;
            }

            context.Value = new ConfigStreamValue
            {
                Data = value,
                IsSuccess = true
            };
        }
    }
}
