using ConfigStream.Api.Startup;

WebApplicationBuilder builder = WebApplication
    .CreateBuilder(args);

builder
    .Services
    .ConfigureServices();

WebApplication app = builder
    .Build()
    .ConfigureWebApplication();

app.MapGroup("api").ConfigureApiEndpoints();
app.MapFallbackToFile("index.html");

app.Run();

//this needed to access from IntegrationTests
public partial class Program { }