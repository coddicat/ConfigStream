using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace ConfigStream.Guid.Utils
{
    public class InvalidatingMemoryCache
    {
        private readonly IMemoryCache _memoryCache;

        public InvalidatingMemoryCache(IMemoryCache memoryCache, IConnectionMultiplexer redis)
        {
            _memoryCache = memoryCache;
            ISubscriber subscriber = redis.GetSubscriber();
            subscriber.Subscribe("invalidate", (x, y) => Invalidate());
        }
        private void Invalidate()
        {

        }
        public object Set(string key, object value) => _memoryCache.Set(key, value);        
        public T Set<T>(string key, T value) => _memoryCache.Set<T>(key, value);
        public object Get(string key) => _memoryCache.Get(key);
        public T Get<T>(string key) => _memoryCache.Get<T>(key);        
    }
}
