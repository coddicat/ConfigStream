﻿using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Handler;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConfigStream.Pipeline
{
    /// <summary>
    /// Handles the writing of Config Values to a local scope-based cache.
    /// This handler is a key component in the configuration stream pipeline, specifically responsible
    /// for updating and storing configuration data in a local cache within a defined scope of a process.    
    /// </summary>
    public class ScopeWriteLocalHandler : IConfigStreamHandler
    {
        private readonly ConcurrentDictionary<string, string> _localCache;

        public ScopeWriteLocalHandler(ScopeLocalCache cache)
        {
            _localCache = cache.Cache;
        }
        public async Task HandleAsync(IHandlerContext context, HandlerDelegate next)
        {
            string key = GetLocalCacheKey(context);

            if (context.Value.IsSuccess)
            {
                _localCache[key] = context.Value.Data;
            }

            await next(context);
        }

        private static string GetLocalCacheKey(IHandlerContext context)
        {
            string key = $"{context.ScopeId}:{context.GroupName}:{context.ConfigName}";
            if (!string.IsNullOrWhiteSpace(context.TargetName))
            {
                key += $":{context.TargetName}";
            }

            return key;
        }        
    }
}
