using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IAuthService
{
    Task<ResponseDto<RegistrationRequestDto>> RegisterStudentOrSupervisor(RegistrationRequestDto request);
    Task<ResponseDto<RegistrationRequestDto>> RegisterAdmin(AdminRegistrationRequestDto registrationRequestDto);
    Task<ResponseDto<AuthResponseDto>> Login(LoginRequestDto request);
    Task<ResponseDto<string>> InitiatePasswordReset(InitiatePasswordResetDto request);
    Task<ResponseDto<string>> ConfirmPasswordReset(ConfirmPasswordResetDto request);
    Task<ResponseDto<AuthResponseDto>>  ConfirmEmail(ConfirmEmailDto request);
    Task<ResponseDto<string>> ResendConfirmationEmail(EmailRequestDto request);
}