using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Enums;

namespace Dissertation.Domain.Entities;

public class StudentInvite : AuditableEntity<long>
{
    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string StudentId { get; set; }

    public string Email { get; set; }

    public string InvitationCode { get; set; }

    public DateTime ExpiryDate { get; set; }

    private StudentInvite(
        string lastName,
        string firstName,
        string studentId,
        string email,
        string invitationCode
    )
    {
        LastName = lastName;
        FirstName = firstName;
        StudentId = studentId;
        Email = email;
        InvitationCode = invitationCode;
        ExpiryDate = DateTime.Today.Add(TimeSpan.FromDays(7)).Date;
    }

    public static StudentInvite Create(
        string lastName,
        string firstName,
        string studentId,
        string email,
        string invitationCode) =>
        new(lastName, firstName, studentId, email, invitationCode);
}