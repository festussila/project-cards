using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

using Asp.Versioning.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Cards.WebAPI.Models.Swagger;

/// <summary>
/// Configures the Swagger generation options.
/// </summary>
/// <remarks>This allows API versioning to define a Swagger document per API version after the
/// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
public class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        // add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        StringBuilder text = new("Describes endpoints to create and manage tasks in the form of cards.");

        OpenApiInfo info = new()
        {
            Title = "Card RESTful APIs",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        info.Description = text.ToString();
        return info;
    }
}