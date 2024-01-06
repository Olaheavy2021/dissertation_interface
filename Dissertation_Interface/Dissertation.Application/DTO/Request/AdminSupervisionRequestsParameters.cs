using Shared.Enums;
using Shared.Helpers;

namespace Dissertation.Application.DTO.Request;

public class AdminSupervisionRequestParameters : PaginationParameters
{
    public long DissertationCohortId { get; set; }

    public SupervisionRequestStatus FilterByStatus { get; set; }

    public string SearchBySupervisor { get; set; } = string.Empty;
}