using Shared.Helpers;

namespace Shared.DTO;

public class SupervisionCohortListParameters : PaginationParameters
{
    public string SearchByUserName { get; set; } = string.Empty;

    public long DissertationCohortId { get; set; }
}