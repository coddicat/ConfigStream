using ConfigStream.Api.Utils;

namespace ConfigStream.Api.Startup
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureWebApplication(this WebApplication webApplication, IConfiguration configuration)
        {
            if (webApplication.Environment.IsDevelopment())
            {
                webApplication
                    .UseCors(options => options
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod());
            }

            bool swagger = configuration.GetValue<bool>("Swagger");
            if (swagger)
            {
                webApplication
                    .UseSwagger()
                    .UseSwaggerUI();
            }

            webApplication
                .UseMiddleware<RequestLoggingMiddleware>()
                //.UseHttpsRedirection()
                //.UseHsts()
                .UseStaticFiles();

            return webApplication;
        }
    }
}
