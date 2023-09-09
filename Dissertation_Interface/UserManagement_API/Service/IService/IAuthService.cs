using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IAuthService
{
    Task<ResponseDto> RegisterStudentOrSupervisor(RegistrationRequestDto registrationRequestDto);

    Task<ResponseDto> RegisterAdmin(AdminRegistrationRequestDto registrationRequestDto);
    Task<ResponseDto> Login(LoginRequestDto loginRequestDto);
}