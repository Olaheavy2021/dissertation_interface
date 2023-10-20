using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.DTO;

namespace UserManagement_API.Extensions;

public static class AuthenticationExtension
{
     public static void SetupAuthentication(this IServiceCollection services, IConfiguration configuration) =>
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"] ?? string.Empty))

            };

            o.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = c => Task.CompletedTask, OnChallenge = HandleFailedAuthentication
            };
        });
    private static Task HandleFailedAuthentication(JwtBearerChallengeContext context)
    {
        var converter = new JsonStringEnumConverter(JsonNamingPolicy.CamelCase);
        var jsonSerializerSettings = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { converter }
        };
        //skip the default logic
        context.HandleResponse();
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var result = new ApplicationProblemDetails((int)HttpStatusCode.Unauthorized)
        {
            Message = context.ErrorDescription
        };

        if (!string.IsNullOrEmpty(context.HttpContext.Request.Headers["Authorization"]))
        {
            context.Response.WriteAsync(JsonSerializer.Serialize(result, jsonSerializerSettings));
            return Task.CompletedTask;
        }

        result.Message = "No token";
        context.Response.WriteAsync(JsonSerializer.Serialize(result, jsonSerializerSettings));
        return Task.CompletedTask;
    }
}