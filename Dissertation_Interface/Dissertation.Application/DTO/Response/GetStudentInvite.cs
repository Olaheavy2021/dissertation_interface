using Shared.DTO;
using Shared.Enums;

namespace Dissertation.Application.DTO.Response;

public class GetStudentInvite
{
    public long Id { get; set; }

    public string LastName { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string StudentId { get; set; } = default!;

    public string Email { get; set; } = default!;

    public DissertationConfigStatus Status { get; set; }

    public GetDissertationCohort DissertationCohort { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public void UpdateStatus() => Status = DateTime.UtcNow.Date > ExpiryDate.Date ? DissertationConfigStatus.Expired : DissertationConfigStatus.Active;
}