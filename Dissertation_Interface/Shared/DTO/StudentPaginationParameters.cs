using Shared.Helpers;

namespace Shared.DTO;

public class StudentPaginationParameters : PaginationParameters
{
    public string SearchByUserName { get; set; } = string.Empty;

    public long FilterByCourse { get; set; }

    public long FilterByCohort { get; set; }
}