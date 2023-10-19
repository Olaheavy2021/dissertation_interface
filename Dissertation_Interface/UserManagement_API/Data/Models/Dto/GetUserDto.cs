namespace UserManagement_API.Data.Models.Dto;

public class GetUserDto
{
    public UserDto? User { get; set; }

    public IList<string> Role { get; set; } = default!;

    public bool IsLockedOut { get; set; }
}