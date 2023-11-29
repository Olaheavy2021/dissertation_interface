using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Notification_API.Data;
using Notification_API.Messaging;
using Notification_API.Middleware.Correlation;
using Notification_API.Services;
using Notification_API.Settings;
using Shared.Helpers;
using Shared.Logging;
using Shared.Settings;

namespace Notification_API.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NotificationDatabaseConnectionString")));

        //Singleton Implementation of the email service and audit log service
        var optionBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
        optionBuilder.UseSqlServer(configuration.GetConnectionString("NotificationDatabaseConnectionString"));
        services.AddSingleton(new EmailService(optionBuilder.Options, configuration));
        services.AddSingleton(new AuditLogService(optionBuilder.Options));

        services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
        services.Configure<ServiceBusSettings>(configuration.GetSection("ServiceBusSettings"));
        services.Configure<SendGridSettings>(configuration.GetSection("SendGridSettings"));
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

        //Configure Redis Cache
        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = configuration.GetConnectionString
                ("RedisCacheConnectionString");
            option.InstanceName = "master";
        });
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }

    public static void ConfigureApiVersioning(this IServiceCollection services) =>
        services.AddApiVersioning(setup =>
        {
            setup.DefaultApiVersion = new ApiVersion(1, 0);
            setup.AssumeDefaultVersionWhenUnspecified = true;
            setup.ReportApiVersions = true;
        }).AddMvc();
}