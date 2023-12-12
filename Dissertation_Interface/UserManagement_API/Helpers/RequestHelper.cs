using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace UserManagement_API.Helpers;

public class RequestHelper : IRequestHelper
{
    private readonly HttpClient _httpClient;

    public RequestHelper(IHttpClientFactory httpClientFactory)
    {
        this._httpClient = httpClientFactory.CreateClient("UserApiClient");
        var mediaTypeWithQualityHeaderValue = new MediaTypeWithQualityHeaderValue("application/json");
        this._httpClient.DefaultRequestHeaders.Accept.Add(mediaTypeWithQualityHeaderValue);

    }

    public async Task<string> PostAsync(string url, object payload, IDictionary<string, object>? queryParams = null,
        IDictionary<string, string>? headers = null,
        Shared.Enums.MediaType? mediaType = null)
    {
        HttpRequestMessage request = BuildRequest(HttpMethod.Post, url, payload, queryParams, headers, mediaType);
        HttpResponseMessage response = await MakeRequestAndHandleException(request);
        return await GetResponseContent(response, request);
    }

    public async Task<string> GetAsync(string url, object? payload, IDictionary<string, object>? queryParams = null,
        IDictionary<string, string>? headers = null,
        Shared.Enums.MediaType? mediaType = null)
    {
        HttpRequestMessage request = BuildRequest(HttpMethod.Get, url, null!, queryParams, headers);
        HttpResponseMessage response = await MakeRequestAndHandleException(request);
        return await GetResponseContent(response, request);
    }

    private static async Task<string> GetResponseContent(HttpResponseMessage response, HttpRequestMessage request)
    {
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            return content;
        }

        var ex = new HttpRequestException(content);
        ex.Data.Add("StatusCode", response.StatusCode);
        ex.Data.Add("Url", request.RequestUri?.ToString());
        throw ex;
    }

    private static bool IsValidUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult) &&
        (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

    private string BuildUrl(string url, IDictionary<string, object>? queryParams)
    {
        if (!IsValidUrl(url) && this._httpClient.BaseAddress == null)
        {
            throw new ArgumentException($"Invalid url provided {url}", nameof(url));
        }

        var query = BuildQueryParams(queryParams);
        return string.IsNullOrEmpty(query) ? url : $"{url}?{query}";
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string url, dynamic payload,
        IDictionary<string, object>? queryParams, IDictionary<string, string>? headers, Shared.Enums.MediaType? mediaType = null)
    {
        var requestUrl = BuildUrl(url, queryParams);

        var request = new HttpRequestMessage { Method = method, RequestUri = new Uri(requestUrl) };

        if (payload != null)
        {
            string? json;
            switch (mediaType)
            {
                case Shared.Enums.MediaType.Json:
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    };
                    json = JsonSerializer.Serialize(payload, options);
                    request.Content = payload is StringContent
                        ? payload
                        : new StringContent(json, Encoding.UTF8, "application/json");
                    break;
                case Shared.Enums.MediaType.UrlEncoded:
                    request.Content = payload is FormUrlEncodedContent ? payload : new FormUrlEncodedContent(payload);
                    request.Content.Headers.ContentType =
                        new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");
                    break;
                default:
                    json = JsonSerializer.Serialize(payload);
                    request.Content = payload is StringContent
                            ? payload
                            : new StringContent(json, Encoding.UTF8, "application/json");
                    break;

            }
        }

        if (headers == null)
        {
            return request;
        }

        {
            foreach ((var key, var value) in headers)
            {
                request.Headers.Add(key, value);
            }
        }

        return request;
    }

    private static string BuildQueryParams(IDictionary<string, object>? queryParams)
    {
        if (queryParams is null || !queryParams.Any())
        {
            return string.Empty;
        }

        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        foreach ((var key, var value) in queryParams)
        {
            query.Add(key, value.ToString());
        }

        return query.ToString()!;
    }

    private async Task<HttpResponseMessage> MakeRequestAndHandleException(HttpRequestMessage request)
    {
        HttpResponseMessage response;

        try
        {
            response = await this._httpClient.SendAsync(request);
        }
        catch (HttpRequestException httpRequestException)
        {
            if (httpRequestException.InnerException is not SocketException)
            {
                throw;
            }

            httpRequestException.Data.Add("StatusCode", HttpStatusCode.ServiceUnavailable);
            httpRequestException.Data.Add("Url", request.RequestUri?.ToString());

            throw;
        }
        return response;
    }
}