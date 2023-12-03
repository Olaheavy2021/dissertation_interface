namespace Dissertation.Application.DTO.Request;

public class CreateStudentInviteRequest
{
    public string LastName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string StudentId { get; set; } = default!;
    public string Email { get; set; } = default!;
}