using System.Diagnostics.CodeAnalysis;
using Notification_API.Messaging;

namespace Notification_API.Extensions;

[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    private static IAzureServiceBusConsumer? ServiceBusConsumer { get; set; }

    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        IHostApplicationLifetime? hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife?.ApplicationStarted.Register(OnStart);
        hostApplicationLife?.ApplicationStopping.Register(OnStop);

        return app;
    }

    public static void ConfigureCors(this IApplicationBuilder app) =>
        app.UseCors(policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

    public static void ConfigureEndpoints(this IApplicationBuilder app) =>
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthz"); ;
        });

    private static void OnStop() => ServiceBusConsumer?.Stop();

    private static void OnStart() => ServiceBusConsumer?.Start();
}