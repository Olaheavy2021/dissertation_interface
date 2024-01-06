using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
public class SupervisionRequestController : Controller
{
    private readonly ISupervisionRequestService _supervisionRequestService;

    public SupervisionRequestController(ISupervisionRequestService supervisionRequestService) =>
        this._supervisionRequestService = supervisionRequestService;

    [Authorize(Roles = "Admin, Superadmin")]
    [HttpGet]
    [SwaggerOperation(Summary = "Fetch a List of Supervision Requests")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionRequests([FromQuery] SupervisionRequestPaginationParameters parameters)
    {
        ResponseDto<PaginatedSupervisionRequestListDto> response =
            await this._supervisionRequestService.GetPaginatedListOfSupervisionRequest(parameters);
        return Ok(response);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student")]
    [SwaggerOperation(Summary = "Fetch a List of Supervision Requests for a Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionRequestsForAStudent([FromQuery] SupervisionRequestPaginationParameters parameters)
    {
        ResponseDto<PaginatedSupervisionRequestListDto> response =
            await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForAStudent(parameters);
        return Ok(response);
    }

    [Authorize(Roles = "Supervisor")]
    [HttpGet("supervisor")]
    [SwaggerOperation(Summary = "Fetch a List of Supervision Requests for a Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionRequestsForASupervisor([FromQuery] SupervisionRequestPaginationParameters parameters)
    {
        ResponseDto<PaginatedSupervisionRequestListDto> response =
            await this._supervisionRequestService.GetPaginatedListOfSupervisionRequestForASupervisor(parameters);
        return Ok(response);
    }

    [Authorize(Roles = "Student")]
    [HttpPost("student/cancel")]
    [SwaggerOperation(Summary = "Cancel a Supervision Request")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelSupervisionRequest([FromBody] ActionSupervisionRequest model)
    {
        ResponseDto<string> response =
            await this._supervisionRequestService.CancelSupervisionRequest(model, new CancellationToken());
        return Ok(response);
    }

    [Authorize(Roles = "Supervisor")]
    [HttpPost("supervisor/reject")]
    [SwaggerOperation(Summary = "Reject a Supervision Request")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RejectSupervisionRequest([FromBody] ActionSupervisionRequest model)
    {
        ResponseDto<string> response =
            await this._supervisionRequestService.RejectSupervisionRequest(model, new CancellationToken());
        return Ok(response);
    }

    [Authorize(Roles = "Supervisor")]
    [HttpPost("supervisor/accept")]
    [SwaggerOperation(Summary = "Accept a Supervision Request")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AcceptSupervisionRequest([FromBody] ActionSupervisionRequest model)
    {
        ResponseDto<string> response =
            await this._supervisionRequestService.AcceptSupervisionRequest(model, new CancellationToken());
        return Ok(response);
    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    [SwaggerOperation(Summary = "Initiate  a Supervision Request")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSupervisionRequest([FromBody] CreateSupervisionRequest model)
    {
        ResponseDto<string> response =
            await this._supervisionRequestService.CreateSupervisionRequest(model, new CancellationToken());
        return Ok(response);
    }
}