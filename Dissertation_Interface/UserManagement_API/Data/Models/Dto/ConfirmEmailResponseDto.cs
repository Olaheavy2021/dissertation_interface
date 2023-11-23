using Shared.DTO;

namespace UserManagement_API.Data.Models.Dto;

public class ConfirmEmailResponseDto
{
    public UserDto? User { get; set; }

    public string Role { get; set; } = string.Empty;

    public string PasswordResetToken { get; set; } = string.Empty;
}