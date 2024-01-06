using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
public class SupervisionCohortController : Controller
{
    private readonly ISupervisionCohortService _supervisionCohortService;

    public SupervisionCohortController(ISupervisionCohortService supervisionCohortService) => this._supervisionCohortService = supervisionCohortService;

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Add supervisors to a cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSupervisionCohort([FromBody] CreateSupervisionCohortListRequest model)
    {
        if (model.SupervisionCohortRequests != null)
        {
            ResponseDto<string> response = await this._supervisionCohortService.CreateSupervisionCohort(model, new CancellationToken());
            return Ok(response);
        }

        return Ok(new ResponseDto<string>
        {
            Message = "Invalid Request",
            IsSuccess = false,
            Result = ErrorMessages.DefaultError
        });
    }

    [Authorize(Roles = "Superadmin, Admin, Student, Supervisor")]
    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Supervision Cohorts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedSupervisionCohortListDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionCohorts([FromQuery] SupervisionCohortListParameters parameters)
    {
        ResponseDto<PaginatedSupervisionCohortListDto> response = await this._supervisionCohortService.GetSupervisionCohorts(parameters);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin, Student, Supervisor")]
    [HttpGet("{id:long}")]
    [SwaggerOperation(Summary = "Get Supervision Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetSupervisionCohort>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionCohort(long id)
    {
        ResponseDto<GetSupervisionCohort> response = await this._supervisionCohortService.GetSupervisionCohort(id);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("inactive")]
    [SwaggerOperation(Summary = "Get Supervision Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedSupervisionCohortListDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> GetInActiveSupervisorsForCohort([FromQuery] SupervisionCohortListParameters parameters)
    {
        ResponseDto<PaginatedUserListDto> response = this._supervisionCohortService.GetInActiveSupervisorsForCohort(parameters);
        return Task.FromResult<IActionResult>(Ok(response));
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPost("update-slots")]
    [SwaggerOperation(Summary = "Update Supervision Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedSupervisionCohortListDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateSupervisionSlot([FromBody] UpdateSupervisionCohortRequest request)
    {
        ResponseDto<string> response = await this._supervisionCohortService.UpdateSupervisionSlot(request, new CancellationToken());
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpDelete("{supervisionCohortId:long}")]
    [SwaggerOperation(Summary = "Remove a Supervisor from a Supervision Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteSupervisionCohort([FromRoute] long supervisionCohortId)
    {
        ResponseDto<string> response = await this._supervisionCohortService.DeleteSupervisionCohort(supervisionCohortId, new CancellationToken());
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("metrics/{cohortId:long}")]
    [SwaggerOperation(Summary = "Remove a Supervisor from a Supervision Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<SupervisionCohortMetricsDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionCohortMetrics([FromRoute] long cohortId)
    {
        ResponseDto<SupervisionCohortMetricsDto> response = await this._supervisionCohortService.GetSupervisionCohortMetrics(cohortId);
        return Ok(response);
    }
}