using ConfigStream.Api.Utils;

namespace ConfigStream.Api.Startup
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureWebApplication(this WebApplication webApplication)
        {
            if (webApplication.Environment.IsDevelopment())
            {
                webApplication
                    .UseSwagger()
                    .UseSwaggerUI()
                    .UseCors(options => options
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .AllowAnyMethod());
            }

            webApplication
                .UseMiddleware<RequestLoggingMiddleware>()
                .UseHttpsRedirection()
                .UseHsts()
                .UseStaticFiles();

            return webApplication;
        }
    }
}
