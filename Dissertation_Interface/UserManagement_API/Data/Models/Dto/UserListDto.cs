namespace UserManagement_API.Data.Models.Dto;

public class UserListDto : UserDto
{
    public bool IsLockedOut { get; set; }
}