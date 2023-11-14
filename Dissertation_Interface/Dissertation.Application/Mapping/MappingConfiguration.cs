using System.Reflection;
using Dissertation.Application.DTO.Response;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
namespace Dissertation.Application.Mapping;

public static class MappingConfiguration
{
    public static void AddMapster(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.Default
            .EnumMappingStrategy(EnumMappingStrategy.ByName);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        TypeAdapterConfig<Domain.Entities.AcademicYear, GetAcademicYear>.NewConfig();

        config.Scan(Assembly.GetExecutingAssembly());
    }

}