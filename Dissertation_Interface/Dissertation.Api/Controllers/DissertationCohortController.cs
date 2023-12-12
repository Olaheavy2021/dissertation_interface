using Dissertation.Application.DissertationCohort.Commands.CreateDissertationCohort;
using Dissertation.Application.DissertationCohort.Commands.UpdateDissertationCohort;
using Dissertation.Application.DissertationCohort.Queries.GetActiveDissertationCohort;
using Dissertation.Application.DissertationCohort.Queries.GetById;
using Dissertation.Application.DissertationCohort.Queries.GetListOfDissertationCohort;
using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;

namespace Dissertation_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Request Unsuccessful", typeof(ResponseDto<string>))]
public class DissertationCohortController : Controller
{
    private readonly ISender _sender;

    public DissertationCohortController(ISender sender) => this._sender = sender;

    [HttpPost]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Create Dissertation Cohort")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<ResponseDto<GetDissertationCohort>>))]
    public async Task<IActionResult> CreateDissertationCohort([FromBody] CreateDissertationCohortRequest request)
    {
        var command = new CreateDissertationCohortCommand(request.StartDate, request.EndDate, request.SupervisionChoiceDeadline, request.AcademicYearId);
        ResponseDto<GetDissertationCohort> result = await this._sender.Send(command);
        return Created(result.Result?.Id.ToString()!, result);
    }

    [HttpGet("{dissertationCohortId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Get Dissertation Cohort By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetDissertationCohort>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetDissertationCohortById([FromRoute] long dissertationCohortId)
    {
        var query = new GetDissertationCohortByIdQuery(dissertationCohortId);
        ResponseDto<GetDissertationCohort> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpGet("active")]
    [Authorize(Roles = "Superadmin,Admin, Student, Supervisor")]
    [SwaggerOperation(Summary = "Get Active Dissertation Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetDissertationCohort>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetActiveDissertationCohort()
    {
        var query = new GetActiveDissertationCohortQuery();
        ResponseDto<GetDissertationCohort> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Get List of Dissertation Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedDissertationCohortListDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfDissertationCohort([FromQuery] DissertationCohortPaginationParameters paginationParameters)
    {
        var query = new GetListOfDissertationCohortQuery(paginationParameters);
        ResponseDto<PaginatedDissertationCohortListDto> response = await this._sender.Send(query);
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

    [HttpPut("{dissertationCohortId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Update Dissertation Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetDissertationCohort>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> UpdateDissertationCohort([FromBody] CreateDissertationCohortRequest request, [FromRoute] long dissertationCohortId)
    {
        var command = new UpdateDissertationCohortCommand(request.StartDate, request.EndDate, request.SupervisionChoiceDeadline, request.AcademicYearId, dissertationCohortId);
        ResponseDto<GetDissertationCohort> response = await this._sender.Send(command);
        return Ok(response);
    }
}