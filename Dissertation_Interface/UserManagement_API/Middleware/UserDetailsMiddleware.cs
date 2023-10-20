using System.IdentityModel.Tokens.Jwt;

namespace UserManagement_API.Middleware;

public class UserDetailsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserDetailsMiddleware> _logger;

    public UserDetailsMiddleware(RequestDelegate next, ILogger<UserDetailsMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].ToString()?.Split("Bearer ")?.LastOrDefault();

        if (!string.IsNullOrEmpty(token))
        {
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