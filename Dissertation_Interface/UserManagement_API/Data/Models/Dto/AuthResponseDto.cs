namespace UserManagement_API.Data.Models.Dto;

public class AuthResponseDto
{
    public UserDto? User { get; set; }
    public string Token { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}