using System.Diagnostics.CodeAnalysis;

namespace Notification_API.Data.Models.Dto;

[ExcludeFromCodeCoverage]
public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}