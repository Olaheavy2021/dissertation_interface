using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using Dissertation.Application.Logger;
using Microsoft.Net.Http.Headers;
using System.Text.Json;
using Dissertation.Application.Extensions;

namespace Dissertation_API.Middleware;

public class HttpLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private const string Redacted = "[Redacted]";

    private readonly HashSet<string> _ignorePaths = new()
    {
        "/healthz",
        "/livez",
        "/metrics",
        "/swagger/index.html",
        "/swagger/v1/swagger.json",
        "/swagger/swagger-ui.css",
        "/swagger/swagger-ui-standalone-preset.js"
    };

    private readonly HashSet<string> _redactHeaders;
    private readonly HashSet<string> _redactBodyFields;
    private readonly ILogger<HttpLoggingMiddleware> _logger;

    public HttpLoggingMiddleware(RequestDelegate next, ILogger<HttpLoggingMiddleware> logger)
    {
        this._next = next;
        this._redactHeaders = new HashSet<string> { HeaderNames.Authorization, HeaderNames.Cookie };
        this._redactBodyFields = new HashSet<string>();
        this._logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (this._ignorePaths.Contains(context.Request.Path))
        {
            await this._next(context);
            return;
        }

        var timer = Stopwatch.StartNew();
        DateTime timestamp = DateTime.UtcNow;

        var requestBody = await GetRequestBodyAsync(context);
        var redactedRequestBody = RedactSensitiveInformation(requestBody, this._redactBodyFields);
        var requestLog = new RequestLog(context.Request.Method, context.Request.Path, redactedRequestBody,
            context.Request.ContentType, GetQueryString(context), FilterHeaders(context.Request.Headers));

        var responseBody = await GetResponseBodyAsync(context);
        var redactedResponseBody = RedactSensitiveInformation(responseBody, this._redactBodyFields);
        var responseLog = new ResponseLog(redactedResponseBody, context.Response.ContentType, FilterHeaders(context.Response.Headers));

        var httpLog = new HttpLog(requestLog, responseLog, context.Response.StatusCode, timestamp,
            timer.ElapsedMilliseconds, GetMetaData(context));
        this._logger.LogInformation("Request and Response Log - {@HttpLog}", httpLog);
    }

    private IReadOnlyDictionary<string, string> FilterHeaders(IHeaderDictionary headers)
    {
        var filteredHeaders = new Dictionary<string, string>();
        foreach (var (key, value) in headers)
        {
            if (this._redactHeaders.Contains(key))
            {
                // Key is among the redacted headers.
                filteredHeaders.Add(key, Redacted);
                continue;
            }
            filteredHeaders.Add(key, value.ToString());
        }


        return filteredHeaders;
    }

    private static async Task<string> GetRequestBodyAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        using var streamReader = new StreamReader(context.Request.Body, leaveOpen: true);
        var requestBody = await streamReader.ReadToEndAsync();
        context.Request.Body.Position = 0;
        return requestBody;
    }


    private static IReadOnlyDictionary<string, string?[]> GetQueryString(HttpContext context) =>
        context.Request.Query
            .ToDictionary(q => q.Key, q => q.Value.ToArray());


    private async Task<string> GetResponseBodyAsync(HttpContext context)
    {
        Stream originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await this._next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var streamReader = new StreamReader(context.Response.Body);
        var responseBodyText = await streamReader.ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
        return responseBodyText;
    }

     private static IReadOnlyDictionary<string, string> GetMetaData(HttpContext context)
   {
       var metaData = new Dictionary<string, string>
       {
           {"trace_id", context.TraceIdentifier}
       };

       metaData.AddIfNotNull("client_id", GetClientId(context));
       metaData.AddIfNotNull("ip_address", context.Connection.RemoteIpAddress?.ToString());
       return metaData;
   }


   private static string? GetClientId(HttpContext context)
   {
       const string type = "clientId";
       var clientId = context.User.FindFirst(type)?.Value;
       if (!string.IsNullOrEmpty(clientId)) return clientId;

       if (!AuthenticationHeaderValue.TryParse(context.Request.Headers.Authorization, out AuthenticationHeaderValue? headerValue))
           return null;

       if (string.IsNullOrEmpty(headerValue.Parameter)) return null;

       var tokenHandler = new JwtSecurityTokenHandler();
       if (!tokenHandler.CanReadToken(headerValue.Parameter)) return null;

       JwtSecurityToken? securityToken = tokenHandler.ReadJwtToken(headerValue.Parameter);
       clientId = securityToken.Claims.FirstOrDefault(c => c.Type == type)?.Value;
       return clientId ?? null;
   }


   private static string RedactSensitiveInformation(string body, HashSet<string> fieldsToRedact)
   {
       try
       {
           if (!string.IsNullOrEmpty(body))
           {
               JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(body);


               if (jsonElement.ValueKind == JsonValueKind.Object)
               {
                   JsonElement.ObjectEnumerator objectEnumerator = jsonElement.EnumerateObject();
                   var updatedProperties = new Dictionary<string, JsonElement>();


                   foreach (JsonProperty property in objectEnumerator)
                   {
                       if (fieldsToRedact.Contains(property.Name))
                       {
                           updatedProperties.Add(property.Name, JsonDocument.Parse($"\"{Redacted}\"").RootElement);
                       }
                       else
                       {
                           updatedProperties.Add(property.Name, property.Value);
                       }
                   }


                   var redactedJson = JsonSerializer.Serialize(updatedProperties);
                   return redactedJson;
               }
           }

           // If the body is not an object, return it as is
           return body;
       }
       catch (JsonException)
       {
           // If the body is not in a valid JSON format, return it as is
           return body;
       }
   }
}