using ConfigStream.Abstractions.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigStream.Pipeline
{
    public static class ScopeReadLocalHandlerExtensions
    {
        public static IPipelineBuilder UseScopeReadLocalHandler(this IPipelineBuilder builder)
        {
            builder.Use<ScopeReadLocalHandler>();
            builder.Services.AddScoped((_) => new ScopeLocalCache());
            return builder;
        }
        public static IPipelineBuilder UseScopeWriteLocalHandler(this IPipelineBuilder builder)
        {
            builder.Use<ScopeWriteLocalHandler>();
            builder.Services.AddScoped((_) => new ScopeLocalCache());
            return builder;
        }
    }
}
