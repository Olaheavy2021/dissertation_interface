using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Notification_API.Data.Models;
using Notification_API.Data.Models.Dto;
using Notification_API.Services;
using Shared.DTO;
using Shared.Helpers;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(Summary = "Get List of Audit Logs")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Processed", typeof(ResponseDto<PagedList<AuditLog>>))]
    public async Task<IActionResult> GetAuditLogs([FromQuery] AuditLogPaginationParameters parameters)
    {
        ResponseDto<PagedList<AuditLog>> response = await this._auditLogService.GetListOfAuditLogs(parameters);

        if (response.Result != null)
        {
            var metadata = new
            {
                response.Result.TotalCount,
                response.Result.PageSize,
                response.Result.CurrentPage,
                response.Result.TotalPages,
                response.Result.HasNext,
                response.Result.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        }
        return Ok(response);
    }

    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get Details of a Audit Log")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Processed", typeof(ResponseDto<AuditLog>))]
    public async Task<IActionResult> GetAuditLog(long id)
    {
        ResponseDto<AuditLog> response = await this._auditLogService.GetAuditLog(id);
        return Ok(response);
    }
}