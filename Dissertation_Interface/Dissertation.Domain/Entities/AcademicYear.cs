using Dissertation.Domain.DomainHelper;

namespace Dissertation.Domain.Entities;

public class AcademicYear : AuditableEntity<long>
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    private AcademicYear(
        DateTime startDate,
        DateTime endDate
        )
    {
        EndDate = endDate.Date;
        StartDate = startDate.Date;
    }

    public static AcademicYear Create(DateTime startDate, DateTime endDate) =>
        new(startDate, endDate);
}