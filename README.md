# ConfigStream for .NET
ConfigStream is a dynamic configuration management library for .NET, tailored for distributed microservices. It enables real-time, consistent, and flexible configuration handling, ideal for environments where settings need to be shared or can change dynamically at runtime.

## Features
- Dynamic and flexible configuration management for microservices.
- Scope-based consistency for configuration values.
- Extensible to various external storage sources (Redis, Elasticsearch, EF).
- Customizable configuration read/write pipelines.

## Usage

```csharp
using ConfigStream;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddConfigStream(builder => builder.ConfigureConfigStreamBuilder());

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

```

## Customization
Developers have the flexibility to decide their configurations, choose handlers from available packages (Redis, Elasticsearch, EF), or write custom pipelines.

## Extending ConfigStream
You can extend ConfigStream by implementing custom handlers or utilizing existing handlers for Redis, Elasticsearch, and EF, allowing for tailored configuration management.