namespace Shared.DTO;

public class StudentOrSupervisorRegistrationDto
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    public long? CourseId { get; set; }

    public long? DepartmentId { get; set; }
}