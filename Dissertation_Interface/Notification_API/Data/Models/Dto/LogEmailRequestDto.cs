using System.Diagnostics.CodeAnalysis;

namespace Notification_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class LogEmailRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmailType { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;
}