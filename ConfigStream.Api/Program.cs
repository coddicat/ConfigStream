using ConfigStream.Admin.Redis;
using ConfigStream.Admin.Redis.Entities;
using ConfigStream.Admin.Redis.Models;
using ConfigStream.Builder;
using ConfigStream.Pipeline;
using ConfigStream.Redis;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000", "http://localhost:5173") // Replace with your frontend origin
              .AllowAnyMethod()
              .AllowAnyHeader()
              //.WithExposedHeaders("New-Authorization")
              .AllowCredentials());
});


services.AddEndpointsApiExplorer();

services.AddSwaggerGen();
services.AddSingleton<IConnectionMultiplexer>((_) => ConnectionMultiplexer.Connect("localhost:6379, defaultDatabase=0"));


services.AddScoped<IAdminConfig, AdminConfig>();

services.AddConfigStream((builder) => 
{
    builder
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
});

services.AddScoped((serviceProvider) =>
{
    IConnectionMultiplexer connection = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
    return connection.GetDatabase();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.MapGet("config-stream/config", (IAdminConfig adminConfig) => adminConfig.GetConfigsAsync());
app.MapPut("config-stream/config", (IAdminConfig adminConfig, Config config) => adminConfig.CreateOrUpdateConfigAsync(config));
app.MapDelete("config-stream/config/{groupName}/{configName}", (IAdminConfig adminConfig, string groupName, string configName) => adminConfig.DeleteConfigAsync(groupName, configName));

app.MapGet("config-stream/value", (IAdminConfig adminConfig) => adminConfig.GetValuesAsync());
app.MapPut("config-stream/value", (IAdminConfig adminConfig, SubmitConfigValue[] configValues) => adminConfig.SetConfigValueAsync(configValues));

app.Run();

