using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ocelot.Middleware;
using Shared.DTO;

namespace Gateway_Solution.Middleware;

public static class OcelotConfigurator
{
    public static OcelotPipelineConfiguration CreateConfiguration()
    {
        var configuration = new OcelotPipelineConfiguration()
        {
            PreErrorResponderMiddleware = async (ctx, next) =>
            {
                var converter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
                var jsonSerializerSettings = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { converter }
                };
                await next.Invoke(); // Call the next middleware first
                switch (ctx.Response.StatusCode)
                {
                    case (int)HttpStatusCode.Unauthorized:
                        await HandleUnauthorizedResponse(ctx, jsonSerializerSettings);
                        break;
                    default:
                        await HandleOtherExceptions(ctx, jsonSerializerSettings);
                        break;
                }

            }
        };

        return configuration;
    }

    private static async Task HandleUnauthorizedResponse(HttpContext ctx, JsonSerializerOptions settings)
    {
        // Check if the response is Unauthorized
        if (ctx.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
        {
            ApplicationProblemDetails result;

            if (!string.IsNullOrEmpty(ctx.Request.Headers["Authorization"]))
            {
                result = new ApplicationProblemDetails((int)HttpStatusCode.Unauthorized)
                {
                    Message = "The provided token is invalid." // Or fetch specific error details if available
                };
            }
            else
            {
                result = new ApplicationProblemDetails((int)HttpStatusCode.Unauthorized)
                {
                    Message = "No token"
                };
            }

            ctx.Response.ContentType = MediaTypeNames.Application.Json;
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(result, settings));
        }
    }

    private static async Task HandleOtherExceptions(HttpContext ctx, JsonSerializerOptions settings)
    {
        var result = new ApplicationProblemDetails(ctx.Response.StatusCode);
        result.Message = result.Name;

        ctx.Response.ContentType = MediaTypeNames.Application.Json;
        await ctx.Response.WriteAsync(JsonSerializer.Serialize(result, settings));

    }
}