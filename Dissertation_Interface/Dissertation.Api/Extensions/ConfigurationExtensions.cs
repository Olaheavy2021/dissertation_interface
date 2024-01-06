using Dissertation.Infrastructure.Messaging;
using Dissertation_API.Middleware;
using Dissertation_API.Middleware.Correlation;

namespace Dissertation_API.Extensions;

public static class ConfigurationExtensions
{
    private static IAzureServiceBusConsumer? ServiceBusConsumer { get; set; }
    public static IApplicationBuilder ConfigureCors(this IApplicationBuilder app) =>
        app.UseCors(policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

    public static IApplicationBuilder ConfigureEndpoints(this IApplicationBuilder app) =>
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthz");
        });

    internal static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }

    internal static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionMiddleware>();

    internal static IApplicationBuilder UseSwaggerBasicAuthMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<SwaggerBasicAuthMiddleware>();

    internal static IApplicationBuilder UseUserDetailsMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<UserDetailsMiddleware>();

    internal static IApplicationBuilder UseCorrelationIdMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationIdMiddleware>();

    internal static IApplicationBuilder UseHttpLoggingMiddleware(this IApplicationBuilder app) =>
        app.UseMiddleware<HttpLoggingMiddleware>();

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        IHostApplicationLifetime? hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife?.ApplicationStarted.Register(OnStart);
        hostApplicationLife?.ApplicationStopping.Register(OnStop);

        return app;
    }

    private static void OnStop() => ServiceBusConsumer?.Stop();

    private static void OnStart() => ServiceBusConsumer?.Start();

}