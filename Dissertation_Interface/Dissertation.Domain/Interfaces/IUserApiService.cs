using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace Dissertation.Domain.Interfaces;

public interface IUserApiService
{
    Task<ResponseDto<GetUserDto>> GetUserByEmail(string email);
    Task<ResponseDto<GetUserDto>> GetUserByUserName(string username);
    Task<ResponseDto<string>> RegisterSupervisor(StudentOrSupervisorRegistrationDto model);
    Task<ResponseDto<string>> RegisterStudent(StudentOrSupervisorRegistrationDto model);
    Task<ResponseDto<PaginatedUserListDto>> GetListOfSupervisors(SupervisorPaginationParameters model);
    Task<ResponseDto<PaginatedUserListDto>> GetListOfStudents(DissertationStudentPaginationParameters model);
    Task<ResponseDto<GetUserDto>> GetUserByUserId(string userId);
    Task<ResponseDto<UserDto>> EditStudent(EditStudentRequestDto model);
    Task<ResponseDto<UserDto>> EditSupervisor(EditSupervisorRequestDto model);
    Task<ResponseDto<UserDto>> AssignSupervisorRoleToAdmin(AssignSupervisorRoleRequestDto model);
    Task<ResponseDto<UserDto>> AssignAdminRoleToSupervisor(AssignAdminRoleRequestDto model);
}