namespace Shared.DTO;

public class StudentListDto : UserDto
{
    public bool IsLockedOut { get; set; }

    public bool EmailConfirmed { get; set; }

    public GetCourse Course { get; set; }
}