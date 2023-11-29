using Shared.Helpers;

namespace Dissertation.Domain.Pagination;

public class CoursePaginationParameters : PaginationParameters
{
    public string SearchByName { get; set; } = default!;

    public string FilterByDepartment { get; set; } = default!;
}