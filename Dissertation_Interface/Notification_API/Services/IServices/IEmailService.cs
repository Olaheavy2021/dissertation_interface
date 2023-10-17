using Notification_API.Data.Models.Dto;

namespace Notification_API.Services.IServices;

public interface IEmailService
{
    Task<EmailResponseDto?> SaveAndSendEmail(LogEmailRequestDto request);
    Task UpdateEmailLogger(string emailIdentifier);
}