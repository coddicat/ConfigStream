using System.Collections.Concurrent;

namespace ConfigStream.Pipeline
{
    public class ScopeLocalCache
    {
        public ScopeLocalCache()
        {
            Cache = new ConcurrentDictionary<string, string>();
        }
        public readonly ConcurrentDictionary<string, string> Cache;
    }
}
