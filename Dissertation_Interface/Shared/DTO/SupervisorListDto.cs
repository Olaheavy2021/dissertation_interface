namespace Shared.DTO;

public class SupervisorListDto : UserDto
{
    public bool IsLockedOut { get; set; }

    public bool EmailConfirmed { get; set; }

    public GetDepartment Department { get; set; }
}