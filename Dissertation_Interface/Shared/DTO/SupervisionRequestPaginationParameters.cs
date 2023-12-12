using Shared.Enums;
using Shared.Helpers;

namespace Shared.DTO;

public class SupervisionRequestPaginationParameters : PaginationParameters
{
    public long DissertationCohortId { get; set; }

    public SupervisionRequestStatus FilterByStatus { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public string SupervisorId { get; set; } = string.Empty;

    public string SearchBySupervisor { get; set; } = string.Empty;

    public string SearchByStudent { get; set; } = string.Empty;
}