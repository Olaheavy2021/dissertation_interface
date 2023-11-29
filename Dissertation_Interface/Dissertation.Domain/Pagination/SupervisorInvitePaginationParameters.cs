using Shared.Helpers;

namespace Dissertation.Domain.Pagination;

public class SupervisorInvitePaginationParameters : PaginationParameters
{
    public string SearchByStaffId { get; set; } = default!;
}