using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigStream.Builder
{
    public class ConfigStreamBuilder : IConfigStreamBuilder
    {
        private readonly PipelineBuilder _environmentReadPipelineBuilder;
        private readonly PipelineBuilder _scopeReadPipelineBuilder;
        private readonly PipelineBuilder _scopeWritePipelineBuilder;
        private ScopePropertyResolverDelegate _environmentResolver;
        private ScopePropertyResolverDelegate _scopeIdResolver;
        public ConfigStreamBuilder(IServiceCollection services)
        {
            _environmentReadPipelineBuilder = new PipelineBuilder(services);
            _scopeReadPipelineBuilder = new PipelineBuilder(services);
            _scopeWritePipelineBuilder = new PipelineBuilder(services);
            Services = services;
        }

        public IServiceCollection Services { get; }
        public IConfigStreamBuilder SetScopeReadPipeline(Action<IPipelineBuilder> builder)
        {
            builder(_scopeReadPipelineBuilder);
            return this;
        }
        public IConfigStreamBuilder SetScopeWritePipeline(Action<IPipelineBuilder> builder)
        {
            builder(_scopeWritePipelineBuilder);
            return this;
        }
        public IConfigStreamBuilder SetEnvironmentReadPipeline(Action<IPipelineBuilder> builder)
        {
            builder(_environmentReadPipelineBuilder);
            return this;
        }

        public IConfigStreamBuilder SetEnvironmentNameResolver(Func<IServiceProvider, string> resolver)
        {
            _environmentResolver = (serviceProvider) => resolver(serviceProvider);
            return this;
        }
        public IConfigStreamBuilder SetScopeIdResolver(Func<IServiceProvider, string> resolver)
        {
            _scopeIdResolver = (serviceProvider) => resolver(serviceProvider);
            return this;
        }

        ScopePropertyResolverDelegate DefaultEnvironment = (_) => "Default";
        ScopePropertyResolverDelegate DefaultScopeId = (_) =>
        {
            Guid guid = Guid.NewGuid();
            return Convert.ToBase64String(guid.ToByteArray());
        };

        internal void Build()
        {           
            Services.AddSingleton(new ConfigStreamOptions
            {
                EnvironmentReadHandlerResolver = _environmentReadPipelineBuilder.Build(),
                ScopeReadHandlerResolver = _scopeReadPipelineBuilder.Build(),
                ScopeWriteHandlerResolver = _scopeWritePipelineBuilder.Build(),
                EnvironmentResolver = _environmentResolver ?? DefaultEnvironment,
                ScopeIdResolver = _scopeIdResolver ?? DefaultScopeId
            });
        }        
    }
}
