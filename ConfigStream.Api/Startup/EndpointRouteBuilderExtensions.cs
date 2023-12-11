using ConfigStream.Admin.Redis;
using ConfigStream.Admin.Redis.Entities;
using ConfigStream.Admin.Redis.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace ConfigStream.Api.Startup
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder ConfigureApiEndpoints(this IEndpointRouteBuilder apiRouteBuilder)
        {
            return apiRouteBuilder
                .ConfigEndpoints()
                .ValueEndpoints();
        }

        private static IEndpointRouteBuilder ValueEndpoints(this IEndpointRouteBuilder apiRouteBuilder)
        {
            RouteGroupBuilder valueApi = apiRouteBuilder.MapGroup("value").WithTags("Config Values");
            
            valueApi.MapGet("", (IAdminConfig adminConfig) => adminConfig.GetValuesAsync())
                .WithMetadata(new SwaggerOperationAttribute("Retrieve all Config Values"));

            valueApi.MapPut("", (IAdminConfig adminConfig, SubmitConfigValue[] configValues) => adminConfig.SetConfigValueAsync(configValues))
                .WithMetadata(new SwaggerOperationAttribute("Store Config Values"));

            valueApi.MapGet("{environment}/{groupName}/{configName}", (IAdminConfig adminConfig, string environment, string groupName, string configName)
                => adminConfig.GetValueAsync(groupName, configName, environment))
                .WithMetadata(new SwaggerOperationAttribute("Retrieve single Config Value"));

            valueApi.MapGet("{environment}/{groupName}/{configName}/{target}", (IAdminConfig adminConfig, string environment, string groupName, string configName, string target)
                => adminConfig.GetValueAsync(groupName, configName, environment, target))
                .WithMetadata(new SwaggerOperationAttribute("Retrieve single Config Value for the particualr target"));

            return apiRouteBuilder;
        }

        private static IEndpointRouteBuilder ConfigEndpoints(this IEndpointRouteBuilder apiRouteBuilder)
        {
            RouteGroupBuilder configApi = apiRouteBuilder.MapGroup("config").WithTags("Configs");
            
            configApi.MapGet("", (IAdminConfig adminConfig) => adminConfig.GetConfigsAsync())
                .WithMetadata(new SwaggerOperationAttribute("Retrieve all Configs"));

            configApi.MapPut("", (IAdminConfig adminConfig, Config config) => adminConfig.CreateOrUpdateConfigAsync(config))
                .WithMetadata(new SwaggerOperationAttribute("Create or update Config"));

            configApi.MapGet("{groupName}/{configName}", (IAdminConfig adminConfig, string groupName, string configName) => adminConfig.GetConfigAsync(groupName, configName))
                .WithMetadata(new SwaggerOperationAttribute("Retrieve single Config"));

            configApi.MapDelete("{groupName}/{configName}", (IAdminConfig adminConfig, string groupName, string configName) => adminConfig.DeleteConfigAsync(groupName, configName))
                .WithMetadata(new SwaggerOperationAttribute("Delete Config"));

            return apiRouteBuilder;
        }
    }
}
