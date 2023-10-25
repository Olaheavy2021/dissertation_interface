using Shared.DTO;
using Shared.Helpers;
using UserManagement_API.Data.Models;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IUserService
{
    Task<ResponseDto<GetUserDto>> GetUser(string userId);
    Task<ResponseDto<bool>> LockOutUser(string email, string? loggedInAdminEmail);
    Task<ResponseDto<bool>> UnlockUser(string email, string? loggedInAdminEmail);
    ResponseDto<PagedList<UserListDto>> GetPaginatedAdminUsers(PaginationParameters paginationParameters);

    Task<ResponseDto<EditUserRequestDto>> EditUser(EditUserRequestDto request, string? loggedInAdminEmail);
}