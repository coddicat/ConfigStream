using ConfigStream.Abstractions;
using ConfigStream.Abstractions.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConfigStream.Builder
{
    public static class ServiceCollectionExtensions
    {
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
