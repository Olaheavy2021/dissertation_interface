using System.Text;
using Microsoft.Data.SqlClient;
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
            Email = request.AdminEmail,
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

    public async Task<ResponseDto<PagedList<AuditLog>>> GetListOfAuditLogs(AuditLogPaginationParameters paginationParameters)
    {
        var parametersList = new List<SqlParameter>();
        var sqlQuery = new StringBuilder("SELECT * FROM AuditLogs");

        // Apply search
        if (!string.IsNullOrEmpty(paginationParameters.SearchByEmail))
        {
            sqlQuery.Append(" WHERE Email LIKE @search");
            parametersList.Add(new SqlParameter("@search", $"%{paginationParameters.SearchByEmail}%"));
        }

        // Apply filter
        if (!string.IsNullOrEmpty(paginationParameters.FilterByEventType))
        {
            var whereOrAnd = sqlQuery.ToString().Contains("WHERE") ? "AND" : "WHERE";
            sqlQuery.Append(" {whereOrAnd} EventType = @filter");
            parametersList.Add(new SqlParameter("@filter", paginationParameters.FilterByEventType));
        }

        await using var db = new NotificationDbContext(this._dbOptions);
        var auditLogs = PagedList<AuditLog>.ToPagedList(
            db.Set<AuditLog>().FromSqlRaw(sqlQuery.ToString(), parametersList.ToArray<object>()).OrderBy(x => x.EventTimeStamp), paginationParameters.PageNumber,
            paginationParameters.PageSize);

        var response = new ResponseDto<PagedList<AuditLog>>
        {
            IsSuccess = true,
            Message = SuccessMessages.DefaultSuccess,
            Result = auditLogs
        };

        return response;
    }

    public async Task<ResponseDto<AuditLog>> GetAuditLog(long userId)
    {
        var response = new ResponseDto<AuditLog>();
        await using var db = new NotificationDbContext(this._dbOptions);
        AuditLog? auditLog = await db.AuditLogs.FirstOrDefaultAsync(x => x.Id == userId);

        if (auditLog != null)
        {
            response.IsSuccess = true;
            response.Message = SuccessMessages.DefaultSuccess;
            response.Result = auditLog;

            return response;
        }

        response.IsSuccess = false;
        response.Message = ErrorMessages.DefaultError;
        return response;
    }
}