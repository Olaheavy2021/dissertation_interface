using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Data.Models.MappingProfiles;

[ExcludeFromCodeCoverage]
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserDto, ApplicationUser>().ReverseMap();
        CreateMap<SupervisorListDto, ApplicationUser>().ReverseMap();
        CreateMap<ProfilePicture, GetProfilePicture>().ReverseMap();
    }
}