using Dissertation.Application.Course.Commands.CreateCourse;
using Dissertation.Application.Course.Commands.DisableCourse;
using Dissertation.Application.Course.Commands.EnableCourse;
using Dissertation.Application.Course.Commands.UpdateCourse;
using Dissertation.Application.Course.Queries.GetAllCourses;
using Dissertation.Application.Course.Queries.GetById;
using Dissertation.Application.Course.Queries.GetListOfActiveCourse;
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


[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized Request", typeof(ResponseDto<string>))]
public class CourseController : Controller
{
    private readonly ISender _sender;

    public CourseController(ISender sender) => this._sender = sender;

    [HttpPost]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Create Course")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<GetCourse>))]
    public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequest request)
    {
        var command = new CreateCourseCommand(request.Name, request.DepartmentId);
        ResponseDto<GetCourse> result = await this._sender.Send(command);
        return Created(result.Result?.Id.ToString()!, result);
    }

    [HttpPost("enable/{courseId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Enable Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetCourse>))]
    public async Task<IActionResult> EnableCourse([FromRoute] long courseId)
    {
        var command = new EnableCourseCommand(courseId);
        ResponseDto<GetCourse> result = await this._sender.Send(command);
        return Ok(result);
    }

    [HttpPost("disable/{courseId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Disable Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetCourse>))]
    public async Task<IActionResult> DisableCourse([FromRoute] long courseId)
    {
        var command = new DisableCourseCommand(courseId);
        ResponseDto<GetCourse> result = await this._sender.Send(command);
        return Ok(result);
    }

    [HttpGet("{courseId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
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
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Get Paginated List of Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedCourseListDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetPaginatedListOfCourse([FromQuery] CoursePaginationParameters paginationParameters)
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
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Update Course")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetCourse>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> UpdateCourse([FromBody] CreateCourseRequest request, [FromRoute] long courseId)
    {
        var query = new UpdateCourseCommand(courseId, request.Name, request.DepartmentId);
        ResponseDto<GetCourse> response = await this._sender.Send(query);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpGet("active")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Get List Of Active Courses")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<IEnumerable<GetCourse>>))]
    public async Task<IActionResult> GetListOfActiveCourses()
    {
        var query = new GetListOfActiveCoursesQuery();
        ResponseDto<IEnumerable<GetCourse>> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpGet("all-courses")]
    [Authorize(Roles = "Superadmin,Admin,Student, Supervisor")]
    [SwaggerOperation(Summary = "Get All Courses")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof( ResponseDto<IReadOnlyList<GetCourse>>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetAllCourses()
    {
        var query = new GetAllCoursesQuery();
        ResponseDto<IReadOnlyList<GetCourse>> response = await this._sender.Send(query);
        return Ok(response);
    }
}