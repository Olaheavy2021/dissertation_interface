using Shared.Enums;
using UserManagement_API.Data.Models.Dto;

namespace Shared.DTO;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;

    public UserStatus Status { get; set; }

    public object? ProfilePicture { get; set; } = null!;

    public void UpdateStatus(DateTimeOffset? lockoutEnd, bool emailConfirmed) =>
        Status = lockoutEnd >= DateTimeOffset.UtcNow
            ? UserStatus.Deactivated
            : emailConfirmed
                ? UserStatus.Active
                : UserStatus.Inactive;
}