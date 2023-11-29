using System.ComponentModel.DataAnnotations.Schema;
using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Enums;

namespace Dissertation.Domain.Entities;

public class DissertationCohort: AuditableEntity<long>
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime SupervisionChoiceDeadline { get; set; }

    public long AcademicYearId { get; set; }

    [ForeignKey("AcademicYearId")]
    public AcademicYear AcademicYear { get; set; }

    private DissertationCohort(
        DateTime startDate,
        DateTime endDate,
        DateTime supervisionChoiceDeadline,
        long academicYearId)
    {
        EndDate = endDate.Date;
        StartDate = startDate.Date;
        SupervisionChoiceDeadline = supervisionChoiceDeadline;
        AcademicYearId = academicYearId;
    }

    public static DissertationCohort Create(DateTime endDate, DateTime startDate, DateTime supervisionChoiceDeadline,  long academicYearId) =>
        new(startDate, endDate, supervisionChoiceDeadline, academicYearId);
}