using Shared.Enums;

namespace Shared.DTO;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? UserName { get; set; } = string.Empty;

    public string? ProfilePicture { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
}