using Shared.DTO;
using UserManagement_API.Data.Models.Dto;

namespace UserManagement_API.Service.IService;

public interface IAuthService
{
    Task<ResponseDto<RegistrationRequestDto>> RegisterStudentOrSupervisor(RegistrationRequestDto request);
    Task<ResponseDto<string>> RegisterAdmin(AdminRegistrationRequestDto registrationRequestDto,  string? loggedInAdminEmail);
    Task<ResponseDto<AuthResponseDto>> Login(LoginRequestDto request);
    Task<ResponseDto<string>> InitiatePasswordReset(InitiatePasswordResetDto request);
    Task<ResponseDto<string>> ConfirmPasswordReset(ConfirmPasswordResetDto request);
    Task<ResponseDto<ConfirmEmailResponseDto>> ConfirmEmail(ConfirmEmailRequestDto request);
    Task<ResponseDto<string>> ResendConfirmationEmail(EmailRequestDto request, string? loggedInAdminEmail);
    Task<ResponseDto<RefreshTokenDto>> GetRefreshToken(RefreshTokenDto request);

    Task<ResponseDto<string>> Logout();
}