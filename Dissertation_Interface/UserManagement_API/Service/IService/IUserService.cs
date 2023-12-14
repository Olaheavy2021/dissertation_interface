using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IUserService
{
    Task<ResponseDto<GetUserDto>> GetUser();
    Task<ResponseDto<GetUserDto>> GetUser(string userId);
    Task<ResponseDto<GetUserDto>> GetUserByEmail(string email);
    Task<ResponseDto<GetUserDto>> GetUserByUserName(string userName);
    Task<ResponseDto<bool>> LockOutUser(string email, string? loggedInAdminEmail);
    Task<ResponseDto<bool>> UnlockUser(string email, string? loggedInAdminEmail);
    ResponseDto<PaginatedUserListDto> GetPaginatedAdminUsers(UserPaginationParameters paginationParameters);
    Task<ResponseDto<PaginatedStudentListDto>> GetPaginatedStudents(
        DissertationStudentPaginationParameters paginationParameters);
    Task<ResponseDto<PaginatedSupervisorListDto>> GetPaginatedSupervisors(
        SupervisorPaginationParameters paginationParameters);
    Task<ResponseDto<EditUserRequestDto>> EditUser(EditUserRequestDto request, string? loggedInAdminEmail);

    Task<ResponseDto<UserDto>> EditSupervisor(EditSupervisorRequestDto request, string? loggedInUser);

    Task<ResponseDto<UserDto>> EditStudent(EditStudentRequestDto request, string? loggedInUser);
}