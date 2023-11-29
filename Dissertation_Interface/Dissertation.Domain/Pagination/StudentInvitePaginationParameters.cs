using Shared.Helpers;

namespace Dissertation.Domain.Pagination;

public class StudentInvitePaginationParameters : PaginationParameters
{
    public string SearchByStudentId { get; set; } = default!;

    public long FilterByCohortId { get; set; } = default;

    public long FilterByCourseId { get; set; } = default;


}