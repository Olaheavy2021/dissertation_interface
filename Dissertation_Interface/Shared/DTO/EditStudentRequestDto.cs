namespace Shared.DTO;

public class EditStudentRequestDto
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string StudentId { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public long CourseId { get; set; }
}