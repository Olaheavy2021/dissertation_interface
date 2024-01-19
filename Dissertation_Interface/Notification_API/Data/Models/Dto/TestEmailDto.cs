using System.Diagnostics.CodeAnalysis;

namespace Notification_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class TestEmailDto
{
    public string EmailBody { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}