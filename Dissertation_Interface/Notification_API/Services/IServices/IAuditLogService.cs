using Notification_API.Data.Models;
using Notification_API.Data.Models.Dto;
using Shared.DTO;
using Shared.Helpers;

namespace Notification_API.Services.IServices;

public interface IAuditLogService
{
    Task<EmailResponseDto> SaveAuditLog(AuditLogDto request);

    Task<ResponseDto<PagedList<AuditLog>>> GetListOfAuditLogs(AuditLogPaginationParameters paginationParameters);

}