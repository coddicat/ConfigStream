using ConfigStream.Abstractions;
using ConfigStream.Builder;
using ConfigStream.Scope;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ConfigStream
{
    public class ConfigStreamService : IConfigStreamService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HandlerDelegate _environmentReadHandler;
        private readonly HandlerDelegate _scopeReadHandler;
        private readonly HandlerDelegate _scopeWriteHandler;
        private readonly ScopePropertyResolverDelegate _scopeIdResolver;

        internal ConfigStreamService(IServiceProvider serviceProvider)
        {
            ConfigStreamOptions options = serviceProvider.GetRequiredService<ConfigStreamOptions>();
            EnvironmentName = options.EnvironmentResolver(serviceProvider);

            _environmentReadHandler = options.EnvironmentReadHandlerResolver(serviceProvider);
            _scopeReadHandler = options.ScopeReadHandlerResolver(serviceProvider);
            _scopeWriteHandler = options.ScopeWriteHandlerResolver(serviceProvider);
            _scopeIdResolver = options.ScopeIdResolver;
            _serviceProvider = serviceProvider;
        }

        public string EnvironmentName { get; }

        public IConfigStreamScope CreateScope(string scopeId = null)
        {
            scopeId = scopeId ?? _scopeIdResolver(_serviceProvider);
            return new ConfigStreamScope(_scopeReadHandler, _scopeWriteHandler, scopeId, EnvironmentName);
        }

        public async Task<ConfigStreamValue> ReadAsync(string groupName, string configName, string targetName = null)
        {
            try
            {
                HandlerContext context = InitHandlerContext(groupName, configName, targetName);
                await _environmentReadHandler(context);
                return context.Value;
            }
            catch (Exception ex)
            {
                return new ConfigStreamValue
                {
                    Exception = ex,
                    IsSuccess = false,
                    Data = null
                };
            }
        }

        private HandlerContext InitHandlerContext(string groupName, string configName, string targetName)
        {
            return new HandlerContext
            {
                EnvironmentName = EnvironmentName,
                ConfigName = configName,
                GroupName = groupName,
                TargetName = targetName,
                Value = new ConfigStreamValue
                {
                    IsSuccess = false,
                    Data = null
                }
            };
        }
    }
}
