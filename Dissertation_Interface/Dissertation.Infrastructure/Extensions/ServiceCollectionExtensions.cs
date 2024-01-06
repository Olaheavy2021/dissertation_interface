using Dissertation.Infrastructure.Context;
using Dissertation.Infrastructure.Messaging;
using Dissertation.Infrastructure.Persistence.IRepository;
using Dissertation.Infrastructure.Persistence.Repositories;
using Dissertation.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Repository;

namespace Dissertation.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        //Database Connection
        services.AddDbContext<DissertationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DissertationDatabaseConnectionString")));
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
        var optionBuilder = new DbContextOptionsBuilder<DissertationDbContext>();
        optionBuilder.UseSqlServer(configuration.GetConnectionString("DissertationDatabaseConnectionString"));
        services.AddSingleton<BatchUploadService>(serviceProvider =>
        {
            IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            DbContextOptions<DissertationDbContext> contextOptions = optionBuilder.Options;
            return new BatchUploadService(contextOptions, configuration, httpContextAccessor);
        });
        return services;
    }
}