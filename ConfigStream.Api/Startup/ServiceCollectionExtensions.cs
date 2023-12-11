using ConfigStream.Admin.Redis;
using StackExchange.Redis;
using ConfigStream.Builder;
using ConfigStream.Redis;
using ConfigStream.Pipeline;
using ConfigStream.Abstractions.Builder;
using Microsoft.AspNetCore.HttpOverrides;

namespace ConfigStream.Api.Startup
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            return services
                .AddHttpContextAccessor()
                .Configure<ForwardedHeadersOptions>(options =>
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto)
                .AddCors()
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(c => {
                    c.EnableAnnotations();
                    c.DocumentFilter<PrefixSwaggerDocumentFilter>();
                })
                .AddSingleton<IConnectionMultiplexer>((_) => ConnectionMultiplexer.Connect("localhost:6379, defaultDatabase=0"))
                .AddScoped<IAdminConfig, AdminConfig>()
                //.AddConfigStream(builder => builder.ConfigureConfigStreamBuilder())
                .AddScoped((serviceProvider) =>
                {
                    IConnectionMultiplexer connection = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
                    return connection.GetDatabase();
                });
        }

        private static IConfigStreamBuilder ConfigureConfigStreamBuilder(this IConfigStreamBuilder builder)
        {
            return builder
                .SetEnvironmentNameResolver(serviceProvider =>
                {
                    IWebHostEnvironment webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                    return webHostEnvironment.EnvironmentName;
                })
                .SetEnvironmentReadPipeline(pipeline =>
                    pipeline.Use<EnvironmentReadRedisHandler>())
                .SetScopeReadPipeline(pipeline =>
                    pipeline
                        .UseScopeReadLocalHandler()
                        .Use<ScopeReadRedisHandler>()
                        .Use<EnvironmentReadRedisHandler>())
                .SetScopeWritePipeline(pipeline =>
                    pipeline
                        .Use<ScopeWriteRedisHandler>()
                        .UseScopeWriteLocalHandler());            
        }
    }
}
