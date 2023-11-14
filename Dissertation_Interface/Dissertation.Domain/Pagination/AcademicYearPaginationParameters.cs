using Shared.Helpers;

namespace Dissertation.Domain.Pagination;

public class AcademicYearPaginationParameters : PaginationParameters
{
    public int SearchByYear { get; set; }

    public string FilterByStatus { get; set; } = string.Empty;

}