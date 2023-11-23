using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Enums;

namespace Dissertation.Domain.Entities;

public class StudentInvite : AuditableEntity<long>
{
    public string LastName { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string StudentId { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Course { get; set; } = default!;
    public DissertationConfigStatus Status { get; set; }

    private StudentInvite(
        string lastName,
        string firstName,
        string studentId,
        string email,
        string course,
        DissertationConfigStatus status
    )
    {
        LastName = lastName;
        FirstName = firstName;
        StudentId = studentId;
        Email = email;
        Course = course;
        Status = status;
    }

    public static StudentInvite Create(
        string lastName,
        string firstName,
        string studentId,
        string email,
        string course) =>
        new(lastName, firstName, studentId, email, course, DissertationConfigStatus.Active);
}