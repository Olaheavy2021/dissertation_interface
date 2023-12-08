using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Application.SupervisionCohort.Commands.CreateSupervisionCohort;
using Dissertation.Application.SupervisionCohort.Queries.GetAssignedSupervisors;
using Dissertation.Application.SupervisionCohort.Queries.GetById;
using Dissertation.Application.SupervisionCohort.Queries.GetUnassignedSupervisors;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;

namespace Dissertation_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized Request", typeof(ResponseDto<string>))]
public class SupervisionCohortController : Controller
{
    private readonly ISender _sender;

    public SupervisionCohortController(ISender sender) => this._sender = sender;

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPost]
    [SwaggerOperation(Summary = "Add supervisors to a cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateSupervisionCohort([FromBody]CreateSupervisionCohort request)
    {
        if (request.SupervisionCohortRequests == null)
        {
            return Ok(new ResponseDto<string>
            {
                Message = "Invalid Request",
                IsSuccess = false,
                Result = ErrorMessages.DefaultError
            });
        }

        var command = new CreateSupervisionCohortCommand(request.SupervisionCohortRequests);
        ResponseDto<string> result = await this._sender.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a SupervisionCohort By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionCohort([FromRoute] long id)
    {
        var query = new GetAssignedSupervisorByIdQuery(id);
        ResponseDto<GetSupervisionCohortDetails> result = await this._sender.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("assigned-supervisors")]
    [SwaggerOperation(Summary = "Get Supervisors that have been assigned to a cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionCohorts([FromQuery] SupervisionCohortListParameters parameters)
    {
        var query = new GetAssignedSupervisorsQuery(parameters);
        ResponseDto<PaginatedSupervisionCohortListDto> result = await this._sender.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("unassigned-supervisors")]
    [SwaggerOperation(Summary = "Get Supervisors that have not been assigned to a cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUnassignedSupervisionCohorts([FromQuery] SupervisionCohortListParameters parameters)
    {
        var query = new GetUnassignedSupervisorsQuery(parameters);
        ResponseDto<PaginatedUserListDto> result = await this._sender.Send(query);
        return Ok(result);
    }
}