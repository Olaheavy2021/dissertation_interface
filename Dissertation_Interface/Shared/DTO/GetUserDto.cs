namespace Shared.DTO;

public class GetUserDto
{
    public UserDto? User { get; set; }

    public IList<string> Role { get; set; } = default!;

    public bool IsLockedOut { get; set; }
}