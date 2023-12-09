using Shared.Enums;

namespace Shared.DTO;

public class GetAcademicYear
{
    public long Id { get; set; }
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DissertationConfigStatus Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public void UpdateStatus() => Status = StartDate.Date <= DateTime.UtcNow.Date && EndDate.Date >= DateTime.UtcNow.Date
        ? DissertationConfigStatus.Active
        : DissertationConfigStatus.InActive;

}