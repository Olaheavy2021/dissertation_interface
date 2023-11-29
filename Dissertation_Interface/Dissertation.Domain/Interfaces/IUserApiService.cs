using Shared.DTO;

namespace Dissertation.Domain.Interfaces;

public interface IUserApiService
{
    Task<ResponseDto<GetUserDto>> GetUserByEmail(string email);

    Task<ResponseDto<GetUserDto>> GetUserByUserName(string username);

    Task<ResponseDto<string>> RegisterSupervisor(StudentOrSupervisorRegistrationDto model);

}