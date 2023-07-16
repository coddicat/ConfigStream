using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigStream.Abstractions.Builder
{
    public interface IConfigStreamBuilder
    {
        IConfigStreamBuilder SetEnvironmentNameResolver(Func<IServiceProvider, string> resolver);
        IConfigStreamBuilder SetScopeIdResolver(Func<IServiceProvider, string> resolver);
        IConfigStreamBuilder SetEnvironmentReadPipeline(Action<IPipelineBuilder> builder);
        IConfigStreamBuilder SetScopeReadPipeline(Action<IPipelineBuilder> builder);
        IConfigStreamBuilder SetScopeWritePipeline(Action<IPipelineBuilder> builder);
        IServiceCollection Services { get; }
    }
}
