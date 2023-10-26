﻿namespace Dissertation_API.Extensions;

public static class ProgramExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) =>
        services
            .AddSwagger()
            .AddServices()
            .ConfigureHealthChecks()
            .ConfigureRedis(config)
            .AddAuth(config)
            .AddConfigurationProps(config)
            .AddCors();

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config) =>
        builder
            .UseRouting()
            .UseSwaggerBasicAuthMiddleware()
            .UseCorrelationIdMiddleware()
            .UseUserDetailsMiddleware()
            .UseExceptionMiddleware()
            .UseAuthentication()
            .UseAuthorization()
            .ConfigureEndpoints()
            .ConfigureCors();
}