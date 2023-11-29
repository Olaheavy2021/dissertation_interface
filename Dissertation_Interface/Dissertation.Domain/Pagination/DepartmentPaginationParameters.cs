using Shared.Helpers;

namespace Dissertation.Domain.Pagination;

public class DepartmentPaginationParameters : PaginationParameters
{
    public string SearchByName { get; set; }

    public string FilterByStatus { get; set; } = string.Empty;
}