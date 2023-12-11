using System.Collections.Concurrent;

namespace ConfigStream.Pipeline
{
    /// <summary>
    /// Represents a local cache specifically for a defined scope within a process.
    /// It is designed to store configuration data for the duration of a specific scope,
    /// identified by a scopeId(correllationId), ensuring isolated and efficient access to the configurations.
    /// </summary>
    public class ScopeLocalCache
    {
        public ScopeLocalCache()
        {
            Cache = new ConcurrentDictionary<string, string>();
        }
        public readonly ConcurrentDictionary<string, string> Cache;
    }
}
