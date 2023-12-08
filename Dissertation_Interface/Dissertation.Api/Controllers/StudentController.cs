using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Application.Student.Commands.RegisterStudent;
using Dissertation.Application.Student.Commands.UpdateStudent;
using Dissertation.Application.Student.Queries.GetAvailableSupervisors;
using Dissertation.Application.Student.Queries.GetListOfStudents;
using Dissertation.Application.Student.Queries.GetStudentById;
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
public class StudentController : Controller
{
    private readonly ISender _sender;

    public StudentController(ISender sender) => this._sender = sender;

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register Student")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentRequest request)
    {
        var command = new RegisterStudentCommand(
            request.FirstName,
            request.LastName,
            request.StudentId,
            request.CourseId,
            request.InvitationCode,
            request.Password
            );

        ResponseDto<string> result = await this._sender.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Students")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedUserListDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfStudents([FromQuery] StudentPaginationParameters paginationParameters)
    {
        var query = new GetListOfStudentsQuery(paginationParameters);
        ResponseDto<PaginatedUserListDto> response = await this._sender.Send(query);

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

    [Authorize(Roles = "Superadmin, Admin, Student, Supervisor")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get Student By UserId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetStudent>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetStudentById([FromRoute] string id)
    {
        var query = new GetStudentByIdQuery(id);
        ResponseDto<GetStudent> response = await this._sender.Send(query);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> EditStudent([FromBody] EditStudentRequestDto request,[FromRoute] long id)
    {
        var query = new UpdateStudentCommand(request.LastName, request.FirstName, request.StudentId, request.CourseId, id);
        ResponseDto<UserDto> response = await this._sender.Send(query);
        return Ok(response);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("available-supervisors")]
    [SwaggerOperation(Summary = "Get Available Supervisor for the Cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetStudent>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetAvailableSupervisors([FromQuery] SupervisionCohortListParameters parameters)
    {
        var query = new GetAvailableSupervisorsQuery(parameters);
        ResponseDto<PaginatedSupervisionCohortListDto> response = await this._sender.Send(query);
        return Ok(response);
    }
}