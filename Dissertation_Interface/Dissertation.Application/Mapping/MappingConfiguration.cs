using System.Reflection;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO;

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
        TypeAdapterConfig<Domain.Entities.Student, StudentMatchingRequest>.NewConfig()
            .Map(dest => dest.StudentTopic, src => src.ResearchTopic);

        config.Scan(Assembly.GetExecutingAssembly());
    }

}