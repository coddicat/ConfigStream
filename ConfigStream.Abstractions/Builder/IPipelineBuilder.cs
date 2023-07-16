using ConfigStream.Abstractions.Handler;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigStream.Abstractions.Builder
{
    public interface IPipelineBuilder
    {
        IServiceCollection Services { get; }
        IPipelineBuilder Use<T>() where T : IConfigStreamHandler;
    }
}
