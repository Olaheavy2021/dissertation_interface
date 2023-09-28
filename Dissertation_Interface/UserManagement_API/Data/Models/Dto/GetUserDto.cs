namespace UserManagement_API.Data.Models.Dto;

public class GetUserDto
{
    public UserDto? User { get; set; }

    public string Role { get; set; } = string.Empty;

    public bool IsLockedOut { get; set; }
}