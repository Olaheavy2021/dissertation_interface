using Shared.Helpers;

namespace Dissertation.Domain.Pagination;

public class DissertationCohortPaginationParameters : PaginationParameters
{
    public int SearchByStartYear { get; set; }
}