using ConfigStream.Admin.Redis;
using ConfigStream.Admin.Redis.Entities;
using ConfigStream.Builder;
using ConfigStream.Pipeline;
using ConfigStream.Redis;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

IServiceCollection services = builder.Services;

services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000") // Replace with your frontend origin
              .AllowAnyMethod()
              .AllowAnyHeader()
              //.WithExposedHeaders("New-Authorization")
              .AllowCredentials());
});


services.AddEndpointsApiExplorer();

services.AddSwaggerGen();
services.AddSingleton<IConnectionMultiplexer>((_) => ConnectionMultiplexer.Connect("localhost"));


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

//services.AddScoped<IConfigStreamService>((serviceProvider) =>
//{
//    IConnectionMultiplexer connection = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
//    IWebHostEnvironment webHostEnvironment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

//    return new ConfigStreamService(connection.GetDatabase(), webHostEnvironment.EnvironmentName);
//});

//services.AddScoped<IConfigScope>((serviceProvider) =>
//{
//    IConfigStreamService configStreamService = serviceProvider.GetRequiredService<IConfigStreamService>();
//    return configStreamService.CreateScope();
//});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();

app.MapGet("config-stream/group", (IAdminConfig adminConfig, string search) => adminConfig.GetGroupsAsync(search));
app.MapPut("config-stream/group", (IAdminConfig adminConfig, ConfigGroup group) => adminConfig.CreateOrUpdateGroupAsync(group));
app.MapDelete("config-stream/group/{groupName}", (IAdminConfig adminConfig, string groupName) => adminConfig.DeleteConfigGroupAsync(groupName));

app.MapGet("config-stream/config", (IAdminConfig adminConfig, string search) => adminConfig.GetConfigsAsync(search));
app.MapPut("config-stream/config", (IAdminConfig adminConfig, Config config) => adminConfig.CreateOrUpdateConfigAsync(config));
app.MapDelete("config-stream/config/{groupName}/{configName}", (IAdminConfig adminConfig, string groupName, string configName) => adminConfig.DeleteConfigAsync(groupName, configName));

app.MapGet("config-stream/value", (IAdminConfig adminConfig, string environment, string search) => adminConfig.GetValuesAsync(environment, search));

app.Run();

