using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Notification_API.Extensions;

public static class SwaggerExtension
{
    public static void SetupSwaggerBearerAuthentication(this IServiceCollection services) =>
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference= new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id=JwtBearerDefaults.AuthenticationScheme
                        }
                    }, Array.Empty<string>()
                }
            });
            option.EnableAnnotations();
        });
}