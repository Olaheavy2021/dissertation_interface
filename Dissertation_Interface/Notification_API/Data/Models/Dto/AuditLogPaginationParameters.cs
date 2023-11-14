using Shared.Helpers;

namespace Notification_API.Data.Models.Dto;

public class AuditLogPaginationParameters : PaginationParameters
{
    public string SearchByEmail { get; set; } = string.Empty;

    public string FilterByEventType { get; set; } = string.Empty;

}