using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigStream.Abstractions.Builder
{
    /// <summary>
    /// Represents a builder for ConfigStream. 
    /// This interface allows for setting up resolvers and defining pipelines 
    /// for reading and writing config values.
    /// </summary>
    public interface IConfigStreamBuilder
    {
        /// <summary>
        /// Sets a resolver for determining the environment name
        /// </summary>
        IConfigStreamBuilder SetEnvironmentNameResolver(Func<IServiceProvider, string> resolver);

        /// <summary>
        /// Sets a resolver for determining the scopeId (correlationId)
        /// </summary>
        IConfigStreamBuilder SetScopeIdResolver(Func<IServiceProvider, string> resolver);

        /// <summary>
        /// Defines a pipeline for reading configuration values in a specific environment
        /// </summary>
        IConfigStreamBuilder SetEnvironmentReadPipeline(Action<IPipelineBuilder> builder);

        /// <summary>
        /// Defines a pipeline for reading configuration values within a particular scope
        /// </summary>
        IConfigStreamBuilder SetScopeReadPipeline(Action<IPipelineBuilder> builder);

        /// <summary>
        /// Defines a pipeline for writing configuration values within a particular scope
        /// </summary>
        IConfigStreamBuilder SetScopeWritePipeline(Action<IPipelineBuilder> builder);

        /// <summary>
        /// Provides access to the service collection
        /// </summary>
        IServiceCollection Services { get; }
    }
}
