using ConfigStream.Admin.Redis;
using StackExchange.Redis;
using ConfigStream.Builder;
using ConfigStream.Redis;
using ConfigStream.Pipeline;
using ConfigStream.Abstractions.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;

namespace ConfigStream.Api.Startup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("Redis");
            ArgumentException.ThrowIfNullOrEmpty(connectionString, nameof(connectionString));
            bool swagger = configuration.GetValue<bool>("Swagger");

            if (swagger)
            {
                services
                .AddHttpContextAccessor()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(c =>
                {
                    c.EnableAnnotations();
                    c.DocumentFilter<PrefixSwaggerDocumentFilter>();
                });
            }

            return services
                .Configure<ForwardedHeadersOptions>(options =>
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto)
                .AddCors()
                .AddSingleton<IConnectionMultiplexer>((provider) => ConnectionMultiplexer.Connect(connectionString))
                .AddScoped<IAdminConfig, AdminConfig>()                
                .AddScoped((serviceProvider) =>
                {
                    IConnectionMultiplexer connection = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
                    return connection.GetDatabase();
                });
        }
    }
}
