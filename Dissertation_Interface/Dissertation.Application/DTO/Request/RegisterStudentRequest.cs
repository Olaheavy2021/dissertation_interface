namespace Dissertation.Application.DTO.Request;

public class RegisterStudentRequest
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string StudentId { get; set; } = default!;
    public long CourseId { get; set; }
    public string InvitationCode { get; set; } = default!;
    public string Password { get; set; } = default!;
}