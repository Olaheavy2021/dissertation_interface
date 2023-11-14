namespace Dissertation.Application.DTO.Request;

public class CreateDissertationCohortRequest
{
    public DateTime StartDate { get; set; }

    public  DateTime EndDate { get; set; }

    public DateTime SupervisionChoiceDeadline { get; set; }

    public long AcademicYearId { get; set; }
}