using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class SupervisorInvite :  AuditableEntity<long>
{
    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string StaffId { get; set; }

    public string Email { get; set; }

    public string Department { get; set; }

    public string InvitationCode { get; set; }

    public DateTime ExpiryDate { get; set; }

    private SupervisorInvite(
        string lastName,
        string firstName,
        string staffId,
        string email,
        string department,
        string invitationCode
    )
    {
        LastName = lastName;
        FirstName = firstName;
        StaffId = staffId;
        Email = email;
        Department = department;
        InvitationCode = invitationCode;
        ExpiryDate = DateTime.Today.Add(TimeSpan.FromDays(7)).Date;
    }

    public static SupervisorInvite Create(
        string lastName,
        string firstName,
        string staffId,
        string email,
        string department,
        string invitationCode
        ) =>
        new(lastName, firstName, staffId, email, department, invitationCode);
}