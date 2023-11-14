using Dissertation.Domain.DomainHelper;
using Dissertation.Domain.Enums;

namespace Dissertation.Domain.Entities;

public class AcademicYear : AuditableEntity<long>
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DissertationConfigStatus Status { get; set; }

    private AcademicYear(
        DateTime startDate,
        DateTime endDate,
        DissertationConfigStatus status)
    {
        EndDate = endDate.Date;
        StartDate = startDate.Date;
        Status = status;
    }

    public static AcademicYear Create(DateTime startDate, DateTime endDate, DissertationConfigStatus status) =>
        new(startDate, endDate, status);
}