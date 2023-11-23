using Dissertation.Domain.Enums;

namespace Dissertation.Application.DTO.Response;

public class GetSupervisorInvite
{
    public long Id { get; set; }

    public string LastName { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string StaffId { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string Department { get; set; } = default!;

    public string InvitationCode { get; set; } = default!;

    public  DissertationConfigStatus Status { get; set; }

    public DateTime ExpiryDate { get; set; }

    public void UpdateStatus() => Status = DateTime.UtcNow.Date > ExpiryDate ? DissertationConfigStatus.Expired : DissertationConfigStatus.Active;
}