namespace UserManagement_API.Data.Models.Dto;

public class AuthResponseDto
{
    public UserDto? User { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}