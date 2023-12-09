using Shared.Enums;

namespace Dissertation.Application.DTO.Response;

public class GetSupervisorInvite
{
    public long Id { get; set; }

    public string LastName { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string StaffId { get; set; } = default!;

    public string Email { get; set; } = default!;

    public DissertationConfigStatus Status { get; set; }

    public DateTime ExpiryDate { get; set; }

    public void UpdateStatus() => Status = DateTime.UtcNow.Date > ExpiryDate.Date ? DissertationConfigStatus.Expired : DissertationConfigStatus.Active;
}