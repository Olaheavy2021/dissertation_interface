using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class SupervisionListController : Controller
{
    private readonly ISupervisionListService _supervisionListService;

    public SupervisionListController(ISupervisionListService supervisionListService) => this._supervisionListService = supervisionListService;

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Supervision Cohorts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedSupervisionCohortListDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionList([FromQuery] SupervisionListPaginationParameters parameters)
    {
        ResponseDto<PaginatedSupervisionListDto> response = await this._supervisionListService.GetPaginatedListOfSupervisionRequest(parameters);
        return Ok(response);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("student")]
    [SwaggerOperation(Summary = "Fetch the Supervisor for a Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionListsForAStudent([FromQuery] SupervisionListPaginationParameters parameters)
    {
        ResponseDto<PaginatedSupervisionListDto> response =
            await this._supervisionListService.GetPaginatedListOfSupervisionListForAStudent(parameters);
        return Ok(response);
    }

    [Authorize(Roles = "Supervisor")]
    [HttpGet("supervisor")]
    [SwaggerOperation(Summary = "Fetch the Supervisor for a Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionListsForASupervisor([FromQuery] SupervisionListPaginationParameters parameters)
    {
        ResponseDto<PaginatedSupervisionListDto> response =
            await this._supervisionListService.GetPaginatedListOfSupervisionListForASupervisor(parameters);
        return Ok(response);
    }
}