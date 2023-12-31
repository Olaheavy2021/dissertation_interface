using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Shared.Helpers;

public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _accessor;

    public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor accessor) => this._accessor = accessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = this._accessor.HttpContext?.Items["AccessToken"] as string;

        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}