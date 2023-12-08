using Dissertation.Domain.Enums;

namespace Shared.DTO;

public class GetDissertationCohort
{
    public long Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime SupervisionChoiceDeadline { get; set; }

    public DissertationConfigStatus Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public GetAcademicYear AcademicYear { get; set; }

    public void UpdateStatus() => Status = StartDate.Date <= DateTime.UtcNow.Date && EndDate.Date >= DateTime.UtcNow.Date
        ? DissertationConfigStatus.Active
        : DissertationConfigStatus.InActive;
}