using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Repository;
using Shared.Settings;
using UserManagement_API.Data;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Repository;
using UserManagement_API.Middleware.Correlation;
using UserManagement_API.Settings;

namespace UserManagement_API;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this  IServiceCollection services, IConfiguration configuration)
    {
        // Appsettings Configuration
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrlSettings"));
        services.Configure<ServiceBusSettings>(configuration.GetSection("ServiceBusSettings"));

        //Database Connection
        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("UserDatabaseConnectionString")));

        // Interfaces and Services
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMessageBus, MessageBus>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddHttpContextAccessor();
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();

        //Swagger Endpoint
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}