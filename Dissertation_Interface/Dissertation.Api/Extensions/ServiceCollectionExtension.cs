﻿using Dissertation_API.Middleware.Correlation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Shared.Helpers;
using Shared.Logging;
using Shared.Settings;

namespace Dissertation_API.Extensions;

public static class ServiceCollectionExtension
{
    internal static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddTransient<IRedisCacheHelper, RedisCacheHelper>()
            .AddTransient<ITokenManager, TokenManager>()
            .AddTransient<IHttpContextAccessor, HttpContextAccessor>()
            .AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>()
            .AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
    internal static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "SHU Dissertation Interface - Dissertation API",
                    Version = "v1",
                    TermsOfService = new Uri("https://www.shu.ac.uk/study-here/terms-and-conditions-and-student-regulations"),
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        {
                            "E-mail",
                            new OpenApiObject
                            {
                                {"url", new OpenApiString("https://www.shu.ac.uk/-/media/home/brand-guidelines/logos/primary-logo-2.jpg?iar=0&hash=8203EF9C1043A40AE3316ABC00EBA24C")},
                                {"altText", new OpenApiString("SHU Logo")}
                            }
                        }
                    }
                });

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

        return services;
    }

    internal static IServiceCollection ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }

    internal static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddHttpContextAccessor()
            .SetupAuthentication(config);
        return services;
    }

    internal static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration config) =>
        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = config.GetConnectionString
                ("RedisCacheConnectionString");
            option.InstanceName = "master";
        });

    internal static IServiceCollection AddConfigurationProps(this IServiceCollection services, IConfiguration config) => services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
}