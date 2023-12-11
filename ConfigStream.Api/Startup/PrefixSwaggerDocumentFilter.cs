using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConfigStream.Api.Startup
{
    public class PrefixSwaggerDocumentFilter : IDocumentFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string XForwardedPrefix = "X-Forwarded-Prefix";

        public PrefixSwaggerDocumentFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            IHeaderDictionary? headers = _httpContextAccessor.HttpContext?.Request.Headers;
            string? prefix = (string?) headers?[XForwardedPrefix];
            if (prefix is null)
            {
                return;
            }
            swaggerDoc.Servers.Add(new OpenApiServer { Url = $"/{prefix}" });
        }
    }
}
