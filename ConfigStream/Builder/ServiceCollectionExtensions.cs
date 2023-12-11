using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigStream.Builder
{
    /// <summary>
    /// Extensions for IServiceCollection to integrate ConfigStream into the service container.
    /// This class provides functionality to register the ConfigStream services and configuration into the dependency injection.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures the ConfigStream services to the specified IServiceCollection.
        /// This method sets up the ConfigStream infrastructure by initializing the ConfigStreamBuilder,
        /// applying the provided configuration builder, and registering the ConfigStreamService.
        /// </summary>
        public static IServiceCollection AddConfigStream(this IServiceCollection services, Action<IConfigStreamBuilder> configBuilder)
        {
            ConfigStreamBuilder builder = new ConfigStreamBuilder(services);
            configBuilder(builder);
            builder.Build();

            services.AddScoped<IConfigStreamService>((serviceProvider) =>
            {
                return new ConfigStreamService(serviceProvider);
            });

            return services;
        }
    }
}
