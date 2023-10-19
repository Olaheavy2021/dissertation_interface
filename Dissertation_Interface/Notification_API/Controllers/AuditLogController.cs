using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notification_API.Data.Models;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;
using Shared.Helpers;

namespace Notification_API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize(Roles = "Admin,Superadmin")]
public class AuditLogController : ControllerBase
{
    private readonly AuditLogService _auditLogService;


    public AuditLogController(AuditLogService auditLogService) => this._auditLogService = auditLogService;

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs([FromQuery] PaginationParameters parameters)
    {
        ResponseDto<PagedList<AuditLog>> response = await this._auditLogService.GetListOfAuditLogs(parameters);
        return Ok(response);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAuditLog(long id)
    {
        ResponseDto<AuditLog> response = await this._auditLogService.GetAuditLog(id);
        return Ok(response);
    }
}