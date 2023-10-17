using Microsoft.EntityFrameworkCore;
using Notification_API.Data;
using Notification_API.Data.Models;
using Notification_API.Data.Models.Dto;
using Notification_API.Services.IServices;
using Shared.Constants;
using Shared.DTO;
using Shared.Helpers;

namespace Notification_API.Services;

public class AuditLogService : IAuditLogService
{
    private readonly DbContextOptions<NotificationDbContext> _dbOptions;

    public AuditLogService(DbContextOptions<NotificationDbContext> dbOptions) => this._dbOptions = dbOptions;

    public async Task<EmailResponseDto> SaveAuditLog(AuditLogDto request)
    {
        var response = new EmailResponseDto { IsSuccess = false, Message = ErrorMessages.DefaultError };
        var auditLog = new AuditLog()
        {
            EventTimeStamp = request.EventTimeStamp,
            EventDescription = request.EventDescription,
            Email = request.Email,
            EventType = request.EventType,
            Outcome = request.Outcome,
            TargetEntity = request.TargetEntity
        };

        await using var db = new NotificationDbContext(this._dbOptions);
        await db.AuditLogs.AddAsync(auditLog);
        await db.SaveChangesAsync();

        response.IsSuccess = true;
        response.Message = SuccessMessages.DefaultSuccess;

        return response;
    }

    public async Task<ResponseDto<PagedList<AuditLog>>> GetListOfAuditLogs(PaginationParameters paginationParameters)
    {
        const string query = @"SELECT * FROM AuditLogs";
        await using var db = new NotificationDbContext(this._dbOptions);
        var auditLogs = PagedList<AuditLog>.ToPagedList(
            db.Set<AuditLog>().FromSqlRaw(query).OrderBy(x => x.EventTimeStamp), paginationParameters.PageNumber,
            paginationParameters.PageSize);
        var response = new ResponseDto<PagedList<AuditLog>>
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess,
            Result = auditLogs
        };

        return response;
    }
}