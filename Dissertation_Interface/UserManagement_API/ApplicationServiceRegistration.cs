using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Logging;
using UserManagement_API.Data;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Repository;

namespace UserManagement_API;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this  IServiceCollection services, IConfiguration configuration)
    {
        //Database Connection
        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("UserDatabaseConnectionString")));

        // Interfaces and Services
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddHttpContextAccessor();

        //Swagger Endpoint
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        return services;
    }
}