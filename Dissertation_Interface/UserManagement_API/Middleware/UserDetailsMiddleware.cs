using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.DTO;
using Shared.Helpers;

namespace UserManagement_API.Middleware;

public class UserDetailsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserDetailsMiddleware> _logger;
    private readonly ITokenManager _tokenManager;

    public UserDetailsMiddleware(RequestDelegate next, ILogger<UserDetailsMiddleware> logger, ITokenManager tokenManager)
    {
        this._next = next;
        this._logger = logger;
        this._tokenManager = tokenManager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString()?.Split("Bearer ")?.LastOrDefault();

        if (!string.IsNullOrEmpty(token) && !context.Request.Path.StartsWithSegments("/swagger") && !token.Contains("Basic"))
        {
            if (!await this._tokenManager.IsCurrentActiveToken())
            {
                var converter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
                var jsonSerializerSettings = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { converter }
                };
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                var result = new ApplicationProblemDetails((int)HttpStatusCode.Unauthorized)
                {
                    Message = "Jwt token has been deactivated."
                };
                this._logger.LogWarning("JWT token has been deactivated for this user, invalid attempt to access resources");
                await context.Response.WriteAsync(JsonSerializer.Serialize(result, jsonSerializerSettings));
                return;
            }
            var handler = new JwtSecurityTokenHandler();

            if (handler.ReadToken(token) is JwtSecurityToken jsonToken)
            {
                var userId = jsonToken.Claims.First(claim => claim.Type == "uid")?.Value;
                var userName = jsonToken.Claims.First(claim => claim.Type == "sub")?.Value;
                var email = jsonToken.Claims.First(claim => claim.Type == "email")?.Value;

                context.Items["UserId"] = userId;
                context.Items["UserName"] = userName;
                context.Items["Email"] = email;
            }
        }

        await this._next(context);
    }

}