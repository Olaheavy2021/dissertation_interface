using Dissertation.Application.AcademicYear.Commands.CreateAcademicYear;
using Dissertation.Application.AcademicYear.Commands.UpdateAcademicYear;
using Dissertation.Application.AcademicYear.Queries.GetActiveAcademicYear;
using Dissertation.Application.AcademicYear.Queries.GetById;
using Dissertation.Application.AcademicYear.Queries.GetListOfAcademicYear;
using Dissertation.Application.DTO;
using Dissertation.Application.DTO.Response;
using Dissertation.Domain.Pagination;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Helpers;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;

namespace Dissertation_API.Controllers;

[Authorize(Roles = "Superadmin,Admin")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Request Unsuccessful", typeof(ResponseDto<string>))]
public class AcademicYearController : Controller
{
    private readonly ISender _sender;

    public AcademicYearController(ISender sender) => this._sender = sender;

    [HttpPost]
    [SwaggerOperation(Summary = "Create Academic Year")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<ResponseDto<GetAcademicYear>>))]
    public async Task<IActionResult> CreateAcademicYear([FromBody]CreateAcademicYearRequest request)
    {
        var command = new CreateAcademicYearCommand(request.StartDate, request.EndDate);
        ResponseDto<GetAcademicYear> result = await this._sender.Send(command);
        return Created(result.Result?.Id.ToString()!, result);
    }

    [HttpGet("{academicYearId:long}")]
    [SwaggerOperation(Summary = "Get Academic Year By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<ResponseDto<GetAcademicYear>>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetAcademicYearById([FromRoute] long academicYearId)
    {
        var query = new GetAcademicYearByIdQuery(academicYearId);
        ResponseDto<GetAcademicYear> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpGet("active")]
    [SwaggerOperation(Summary = "Get Academic Year By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<ResponseDto<string>>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetActiveAcademicYear()
    {
        var query = new GetActiveAcademicYearQuery();
        ResponseDto<GetAcademicYear> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Academic Year")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<ResponseDto<string>>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfAcademicYear([FromQuery]AcademicYearPaginationParameters paginationParameters)
    {
        var query = new GetListOfAcademicYearQuery(paginationParameters);
        ResponseDto<PagedList<GetAcademicYear>> response = await this._sender.Send(query);
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

    [HttpPut("{academicYearId:long}")]
    public async Task<IActionResult> UpdateAcademicYear([FromBody] CreateAcademicYearRequest request, [FromRoute]long academicYearId)
    {
        var command = new UpdateAcademicYearCommand(request.StartDate, request.EndDate, academicYearId);
        ResponseDto<GetAcademicYear> response = await this._sender.Send(command);
        return Ok(response);
    }
}