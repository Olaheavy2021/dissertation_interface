namespace UserManagement_API.Data.Models.Dto;

public class UserListDto : UserDto
{
    public bool IsLockedOut { get; set; }

    public bool EmailConfirmed { get; set; }

    public string Status
    {
        get
        {
            if (IsLockedOut)
            {
                return "Deactivated";
            }

            if (EmailConfirmed)
            {
                return "Active";
            }

            return "Inactive";
        }
    }
}