namespace Shared.DTO;

public class DissertationStudentPaginationParameters : StudentPaginationParameters
{
    public DateTime CohortStartDate { get; set; }

    public DateTime CohortEndDate { get; set; }
}