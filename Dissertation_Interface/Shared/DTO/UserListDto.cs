namespace Shared.DTO;

public class UserListDto : UserDto
{
    public bool IsLockedOut { get; set; }

    public bool EmailConfirmed { get; set; }
}