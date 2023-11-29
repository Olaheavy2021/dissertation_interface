using System.ComponentModel.DataAnnotations.Schema;
using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class StudentInvite : AuditableEntity<long>
{
    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string StudentId { get; set; }

    public string Email { get; set; }

    public string InvitationCode { get; set; }

    public DateTime ExpiryDate { get; set; }

    public long DissertationCohortId { get; set; }

    [ForeignKey("DissertationCohortId")]
    public DissertationCohort DissertationCohort { get; set; }

    private StudentInvite(
        string lastName,
        string firstName,
        string studentId,
        string email,
        string invitationCode,
        long dissertationCohortId
    )
    {
        LastName = lastName;
        FirstName = firstName;
        StudentId = studentId;
        Email = email;
        InvitationCode = invitationCode;
        ExpiryDate = DateTime.Today.Add(TimeSpan.FromDays(7)).Date;
        DissertationCohortId = dissertationCohortId;
    }

    public static StudentInvite Create(
        string lastName,
        string firstName,
        string studentId,
        string email,
        string invitationCode,
        long dissertationCohortId
        ) =>
        new(lastName, firstName, studentId, email, invitationCode, dissertationCohortId);
}