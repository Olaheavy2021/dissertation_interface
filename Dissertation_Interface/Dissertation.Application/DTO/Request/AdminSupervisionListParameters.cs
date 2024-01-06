using Shared.Enums;
using Shared.Helpers;

namespace Dissertation.Application.DTO.Request;

public class AdminSupervisionListParameters : PaginationParameters
{
    public long DissertationCohortId { get; set; }

    public string SearchBySupervisor { get; set; } = string.Empty;
}