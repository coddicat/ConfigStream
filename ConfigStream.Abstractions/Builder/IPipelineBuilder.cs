using ConfigStream.Abstractions.Handler;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigStream.Abstractions.Builder
{
    /// <summary>
    /// Defines a builder for constructing a pipeline of ConfigStream handlers.
    /// This interface allows for the addition of handlers to the pipeline, which
    /// can process configuration data as it is read from or written to various sources.
    /// </summary>
    public interface IPipelineBuilder
    {
        /// <summary>
        /// Provides access to the service collection
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Adds a new handler to the pipeline
        /// </summary>
        IPipelineBuilder Use<T>() where T : IConfigStreamHandler;
    }
}
