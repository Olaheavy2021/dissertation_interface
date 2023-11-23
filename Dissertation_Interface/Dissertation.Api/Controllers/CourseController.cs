using Dissertation.Application.Course.Commands.CreateCourse;
using Dissertation.Application.Course.Commands.DisableCourse;
using Dissertation.Application.Course.Commands.EnableCourse;
using Dissertation.Application.Course.Commands.UpdateCourse;
using Dissertation.Application.Course.Queries.GetById;
using Dissertation.Application.Course.Queries.GetListOfCourse;
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

[Authorize(Roles = "Superadmin,Admin")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized Request", typeof(ResponseDto<string>))]
public class CourseController : Controller
{
    private readonly ISender _sender;

    public CourseController(ISender sender) => this._sender = sender;

    [HttpPost]
    [SwaggerOperation(Summary = "Create Course")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<GetCourse>))]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
    {
        var command = new CreateCourseCommand(request.Name, request.DepartmentId);
        ResponseDto<GetCourse> result = await this._sender.Send(command);
        return Created(result.Result?.Id.ToString()!, result);
    }

    [HttpPost("enable/{courseId:long}")]
    [SwaggerOperation(Summary = "Enable Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetCourse>))]
    public async Task<IActionResult> EnableCourse([FromRoute] long courseId)
    {
        var command = new EnableCourseCommand(courseId);
        ResponseDto<GetCourse> result = await this._sender.Send(command);
        return Ok(result);
    }

    [HttpPost("disable/{courseId:long}")]
    [SwaggerOperation(Summary = "Disable Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetCourse>))]
    public async Task<IActionResult> DisableCourse([FromRoute] long courseId)
    {
        var command = new DisableCourseCommand(courseId);
        ResponseDto<GetCourse> result = await this._sender.Send(command);
        return Ok(result);
    }

    [HttpGet("{courseId:long}")]
    [SwaggerOperation(Summary = "Get Course By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetCourse>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetCourseById([FromRoute] long courseId)
    {
        var query = new GetCourseByIdQuery(courseId);
        ResponseDto<GetCourse> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedCourseListDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfCourse([FromQuery] CoursePaginationParameters paginationParameters)
    {
        var query = new GetListOfCourseQuery(paginationParameters);
        ResponseDto<PaginatedCourseListDto> response = await this._sender.Send(query);
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

    [HttpPut("{courseId:long}")]
    [SwaggerOperation(Summary = "Update Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetCourse>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> UpdateCourse([FromBody] CreateCourseRequest request, [FromRoute]long courseId)
    {
        var query = new UpdateCourseCommand(courseId, request.Name, request.DepartmentId);
        ResponseDto<GetCourse> response = await this._sender.Send(query);
        return Ok(response);
    }
}