using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.DTO;
using Shared.Helpers;

namespace Notification_API.Middleware;

public class TokenManagerMiddleware
{
    private readonly ITokenManager _tokenManager;
    private readonly RequestDelegate _next;

    public TokenManagerMiddleware(ITokenManager tokenManager, RequestDelegate next)
    {
        this._tokenManager = tokenManager;
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (await this._tokenManager.IsCurrentActiveToken())
        {
            await this._next(context);

            return;
        }
        context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        var result = new ApplicationProblemDetails((int)HttpStatusCode.Unauthorized)
        {
            Message = "Jwt token has been deactivated."
        };
        var converter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
        var jsonSerializerSettings = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { converter }
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(result, jsonSerializerSettings));
    }
}