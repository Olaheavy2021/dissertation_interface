using Shared.Helpers;

namespace Shared.DTO;

public class SupervisorPaginationParameters : PaginationParameters
{
    public string SearchByUserName { get; set; } = string.Empty;

    public long FilterByDepartment { get; set; }

    public string LoggedInAdminId { get; set; } = string.Empty;
}