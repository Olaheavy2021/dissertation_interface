using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IAuthService
{
    Task<ResponseDto> RegisterStudentOrSupervisor(RegistrationRequestDto request);
    Task<ResponseDto> RegisterAdmin(AdminRegistrationRequestDto request);
    Task<ResponseDto> Login(LoginRequestDto request);
    Task<ResponseDto> InitiatePasswordReset(InitiatePasswordResetDto request);
    Task<ResponseDto> ConfirmPasswordReset(ConfirmPasswordResetDto request);
    Task<ResponseDto> ConfirmEmail(ConfirmEmailDto request);
}