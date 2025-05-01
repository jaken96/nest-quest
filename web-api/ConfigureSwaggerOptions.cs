// Multiple API versions requires more structured management of Swagger configuration
// NOTE:  this is a work in progress from the linked example:
// https://referbruv.com/blog/integrating-aspnet-core-api-versions-with-swagger-ui/#aioseo-solution-mapping-to-api-versions
namespace NestQuest.Services;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider; // Not sure what this does yet

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        this.provider = provider;  
    }

    public void Configure(SwaggerGenOptions options) 
    {
        // We add a swagger document for every version we have
        foreach (var description in provider.ApiVersionDescriptions)  // provider is part of the class configurator
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description)
            );
        }
    }

    public void Configure(string name, SwaggerGenOptions options) // This is wrong.. why do we accept 'name' and not use it?
    {
        Configure(options);
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Nest Quest, or Athena's Lodge, or something else fun and salient"
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += "WARNING:  This API version has been deprecated.";
        }

        return info;
    }

}