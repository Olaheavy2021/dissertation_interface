using UserManagement_API.Data.Models.Dto;

namespace Shared.DTO;

public class GetUserDto
{
    public UserDto? User { get; set; }

    public IList<string> Role { get; set; } = default!;

    public GetProfilePicture ProfilePicture { get; set; } = null!;

    public bool IsLockedOut { get; set; }
}