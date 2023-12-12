using ConfigStream.Api.Startup;

WebApplicationBuilder builder = WebApplication
    .CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder
    .Services
    .ConfigureServices(configuration);

WebApplication app = builder
    .Build()
    .ConfigureWebApplication(configuration);

app.MapGroup("api").ConfigureApiEndpoints();
app.MapFallbackToFile("index.html");

app.Run();

//this needed to access from IntegrationTests
public partial class Program { }