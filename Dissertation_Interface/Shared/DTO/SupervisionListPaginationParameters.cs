using Shared.Helpers;

namespace Shared.DTO;

public class SupervisionListPaginationParameters : PaginationParameters
{
    public long DissertationCohortId { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public string SupervisorId { get; set; } = string.Empty;

    public string SearchBySupervisor { get; set; } = string.Empty;

    public string SearchByStudent { get; set; } = string.Empty;
}