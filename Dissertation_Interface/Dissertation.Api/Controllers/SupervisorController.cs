using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Application.Supervisor.Commands.AssignAdminRole;
using Dissertation.Application.Supervisor.Commands.AssignSupervisorRole;
using Dissertation.Application.Supervisor.Commands.RegisterSupervisor;
using Dissertation.Application.Supervisor.Commands.UpdateResearchArea;
using Dissertation.Application.Supervisor.Commands.UpdateSupervisor;
using Dissertation.Application.Supervisor.Queries.GetActiveStudentsCohort;
using Dissertation.Application.Supervisor.Queries.GetListOfSupervisors;
using Dissertation.Application.Supervisor.Queries.GetSupervisorById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement_API.Data.Models.Dto;

namespace Dissertation_API.Controllers;


[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized Request", typeof(ResponseDto<string>))]
public class SupervisorController : Controller
{
    private readonly ISender _sender;

    public SupervisorController(ISender sender) => this._sender = sender;

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register Supervisor")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> RegisterSupervisor([FromBody] RegisterSupervisorRequest request)
    {
        var command = new RegisterSupervisorCommand(
            request.FirstName,
            request.LastName,
            request.StaffId,
            request.DepartmentId,
            request.InvitationCode,
            request.Password
            );

        ResponseDto<string> result = await this._sender.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Supervisors")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedUserListDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfSupervisors([FromQuery] SupervisorPaginationParameters paginationParameters)
    {
        var query = new GetListOfSupervisorsQuery(paginationParameters);
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
    [SwaggerOperation(Summary = "Get Supervisor By UserId")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetSupervisor>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetSupervisorById([FromRoute] string id)
    {
        var query = new GetSupervisorByIdQuery(id);
        ResponseDto<GetSupervisor> response = await this._sender.Send(query);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPut("{id:long}")]
    [SwaggerOperation(Summary = "Update Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetStudent>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> EditStudent([FromBody] EditSupervisorRequestDto request,[FromRoute] long id)
    {
        var command = new UpdateSupervisorCommand(request.LastName, request.FirstName, request.StaffId, request.DepartmentId, id);
        ResponseDto<UserDto> response = await this._sender.Send(command);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin")]
    [HttpPost("assign-admin-role")]
    [SwaggerOperation(Summary = "Assign Admin Role to a Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> AssignAdminRoleToSupervisor([FromBody] AssignAdminRoleRequestDto request)
    {
        var command = new AssignAdminRoleCommand(request.Email, request.Role);
        ResponseDto<UserDto> response = await this._sender.Send(command);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin")]
    [HttpPost("assign-supervisor-role")]
    [SwaggerOperation(Summary = "Assign Supervisor role to admin")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> AssignSupervisorRoleToAdmin([FromBody] AssignSupervisorRoleRequestDto request)
    {
        var command = new AssignSupervisorRoleCommand(request.Email, request.DepartmentId);
        ResponseDto<UserDto> response = await this._sender.Send(command);
        return Ok(response);
    }

    [Authorize(Roles = "Supervisor")]
    [HttpGet("available-students")]
    [SwaggerOperation(Summary = "Get Available Students for the active cohort")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> GetAvailableStudents([FromQuery] StudentPaginationParameters parameters)
    {
        var query = new GetActiveStudentsCohortQuery(parameters);
        ResponseDto<PaginatedUserListDto> response = await this._sender.Send(query);
        return Ok(response);
    }

    [Authorize(Roles = "Supervisor")]
    [HttpPut("research-area")]
    [SwaggerOperation(Summary = "Update Research Area for a Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> UpdateResearchArea([FromBody] UpdateResearchAreaRequest request )
    {
        var command = new UpdateResearchAreaCommand(request.ResearchArea);
        ResponseDto<SupervisorDto> response = await this._sender.Send(command);
        return Ok(response);
    }
}