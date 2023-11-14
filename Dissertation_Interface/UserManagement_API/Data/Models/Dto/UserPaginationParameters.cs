using Shared.Helpers;

namespace UserManagement_API.Data.Models.Dto;

public class UserPaginationParameters : PaginationParameters
{
    public string LoggedInAdminId { get; set; } = string.Empty;

    public string SearchByUserName { get; set; } = string.Empty;
}