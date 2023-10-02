using Notification_API.Data.Models.Dto;

namespace Notification_API.Services.IServices;

public interface IEmailService
{
    Task<ResponseDto?> RegisterAdminUserEmailAndLog(string emailBody, string email);

    Task<ResponseDto?> ResetPasswordEmailAndLog(string emailBody, string email);

    Task<ResponseDto?> AccountLockedEmailAndLog(string emailBody, string email);

    Task<ResponseDto?> AccountUnLockedEmailAndLog(string emailBody, string email);

    Task UpdateEmailLogger(string emailIdentifier);
}