using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IProfilePictureService
{
    Task<ResponseDto<string>> UploadProfilePicture(ProfilePictureUploadRequestDto request,
        CancellationToken cancellationToken);
}