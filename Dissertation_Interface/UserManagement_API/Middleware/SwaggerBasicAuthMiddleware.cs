using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Shared.Constants;

namespace UserManagement_API.Middleware;

[ExcludeFromCodeCoverage]
public class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    public SwaggerBasicAuthMiddleware(RequestDelegate next) => this._next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                // Get the credentials from request header
                var header = AuthenticationHeaderValue.Parse(authHeader);
                var inBytes = Convert.FromBase64String(header.Parameter);
                var credentials = Encoding.UTF8.GetString(inBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];
                // validate credentials
                if (username.Equals("swagger")
                    && password.Equals(SystemDefault.DefaultPassword))
                {
                    await this._next.Invoke(context).ConfigureAwait(false);
                    return;
                }
            }
            context.Response.Headers["WWW-Authenticate"] = "Basic";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else
        {
            await this._next.Invoke(context).ConfigureAwait(false);
        }
    }
}