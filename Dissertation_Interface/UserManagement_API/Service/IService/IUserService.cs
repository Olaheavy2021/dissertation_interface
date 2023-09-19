using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IUserService
{
    Task<ResponseDto<List<UserDto>>> GetAdminUsers();

    Task<ResponseDto<GetUserDto>> GetUser(string userId);
}