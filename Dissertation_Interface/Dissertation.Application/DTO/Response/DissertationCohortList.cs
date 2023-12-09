using Shared.Enums;

namespace Dissertation.Application.DTO.Response;

public class DissertationCohortList
{
    public long Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime SupervisionChoiceDeadline { get; set; }
    public DissertationConfigStatus Status { get; set; }
    public long AcademicYearId { get; set; }
    public DateTime AcademicYearStartDate { get; set; }
    public DateTime AcademicYearEndDate { get; set; }
    public DissertationConfigStatus AcademicYearStatus { get; set; }
}