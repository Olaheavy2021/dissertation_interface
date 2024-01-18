using AutoMapper;
using Shared.DTO;

namespace UserManagement_API.Data.Models.MappingProfiles;

public class SupervisionCohortProfile : Profile
{
    public SupervisionCohortProfile() => CreateMap<SupervisionCohort, GetSupervisionCohort>().ReverseMap();
}