using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Shared.BlobStorage;
using Shared.Helpers;
using Shared.Logging;
using Shared.MessageBus;
using Shared.Repository;
using Shared.Settings;
using UserManagement_API.Data;
using UserManagement_API.Data.IRepository;
using UserManagement_API.Data.Repository;
using UserManagement_API.ExternalServices;
using UserManagement_API.Helpers;
using UserManagement_API.Middleware.Correlation;
using UserManagement_API.Service;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Extensions;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // AppSettings Configuration
        services.Configure<ApplicationUrlSettings>(configuration.GetSection("ApplicationUrlSettings"));
        services.Configure<ServiceBusSettings>(configuration.GetSection("ServiceBusSettings"));
        services.Configure<ServiceUrlSettings>(configuration.GetSection(ServiceUrlSettings.SectionName));
        services.Configure<BlobStorageSettings>(configuration.GetSection(BlobStorageSettings.SectionName));

        //Database Connection
        services.AddDbContext<UserDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("UserDatabaseConnectionString")));

        // Interfaces and Services
        services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<BackendApiAuthenticationHttpClientHandler>();
        services.AddScoped<IRequestHelper, RequestHelper>();
        services.AddScoped<IDissertationApiService, DissertationApiService>();
        services.AddScoped<ISupervisionCohortService, SupervisionCohortService>();
        services.AddScoped<ISupervisionListService, SupervisionListService>();
        services.AddScoped<ISupervisionRequestService, SupervisionRequestService>();
        services.AddScoped<IMessageBus, MessageBus>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddHttpContextAccessor();
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
        services.AddSingleton(x => new BlobServiceClient(configuration["BlobStorageSettings:ConnectionString"]));
        services.AddSingleton<IBlobRepository, BlobRepository>();
        services.AddScoped<IProfilePictureService, ProfilePictureService>();

        //Swagger Endpoint
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
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

        //API Versioning
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("api-version"),
                new HeaderApiVersionReader("X-Version"),
                new MediaTypeApiVersionReader("ver"));
        });

        //HealthCheck
        services.AddHealthChecks();

        //Configure Redis Cache
        services.AddStackExchangeRedisCache(option =>
        {
            option.Configuration = configuration.GetConnectionString
                ("RedisCacheConnectionString");
            option.InstanceName = "master";
        });
        services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

        services.AddControllers(options =>
            {
                options.OutputFormatters.RemoveType<StringOutputFormatter>();
                options.ModelValidatorProviders.Clear();
            })
            .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

        services.AddHttpClient("UserApiClient", u => u.BaseAddress =
            new Uri(configuration["ServiceUrls:DissertationApi"] ?? throw new InvalidOperationException())).AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();

        return services;
    }
}