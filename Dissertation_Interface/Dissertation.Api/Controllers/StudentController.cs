using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Application.Student.Commands.CancelSupervisionRequest;
using Dissertation.Application.Student.Commands.InitiateSupervisionRequest;
using Dissertation.Application.Student.Commands.RegisterStudent;
using Dissertation.Application.Student.Commands.UpdateResearchTopic;
using Dissertation.Application.Student.Commands.UpdateStudent;
using Dissertation.Application.Student.Commands.UploadResearchProposal;
using Dissertation.Application.Student.Queries.GetAvailableSupervisors;
using Dissertation.Application.Student.Queries.GetListOfStudents;
using Dissertation.Application.Student.Queries.GetStudentById;
using Dissertation.Application.Student.Queries.GetSupervisionLists;
using Dissertation.Application.Student.Queries.GetSupervisionRequests;
using Dissertation.Application.SupervisorSuggestion.Queries.GetSuggestionsForStudent;
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
        ResponseDto<PaginatedStudentListDto> response = await this._sender.Send(query);

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
    public async Task<IActionResult> EditStudent([FromBody] EditStudentRequestDto request, [FromRoute] long id)
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

    [Authorize(Roles = "Student")]
    [HttpPut("research-topic")]
    [SwaggerOperation(Summary = "Update Research Topic for a Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> UpdateResearchTopic([FromBody] UpdateResearchTopicRequest request)
    {
        var command = new UpdateResearchTopicCommand(request.ResearchTopic);
        ResponseDto<StudentDto> response = await this._sender.Send(command);
        return Ok(response);
    }

    [Authorize(Roles = "Student")]
    [HttpPost("initiate-request")]
    [SwaggerOperation(Summary = "Student Initiates a Supervision Request to a Supervisor")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> InitiateSupervisionRequestStudent([FromBody] CreateSupervisionRequest request)
    {
        var command = new InitiateSupervisionRequestCommand(
          request.SupervisorId
        );
        ResponseDto<string> result = await this._sender.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("supervision-requests")]
    [SwaggerOperation(Summary = "Get Supervision Requests for a Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionRequests([FromQuery] StudentSupervisionRequestParameters parameters)
    {
        var query = new GetStudentSupervisionRequestsQuery(parameters);
        ResponseDto<PaginatedSupervisionRequestListDto> result = await this._sender.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpPut("supervision-requests/cancel")]
    [SwaggerOperation(Summary = "Cancel Supervision Requests")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CancelSupervisionRequest([FromBody] ActionSupervisionRequest request)
    {
        var query = new CancelSupervisionRequestCommand(request.RequestId, request.Comment);
        ResponseDto<string> result = await this._sender.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("supervision-list")]
    [SwaggerOperation(Summary = "Get Supervision List for a Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionList([FromQuery] StudentSupervisionListsParameters parameters)
    {
        var query = new GetStudentSupervisionListsQuery(parameters);
        ResponseDto<PaginatedSupervisionListDto> result = await this._sender.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpPut("upload-research-proposal")]
    [SwaggerOperation(Summary = "Upload Research Proposal")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UploadResearchProposal([FromForm] UploadResearchProposalRequest request)
    {
        var query = new UploadResearchProposalCommand(request.ResearchProposal);
        ResponseDto<string> result = await this._sender.Send(query);
        return Ok(result);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("supervision-suggestions")]
    [SwaggerOperation(Summary = "Get Supervision Suggestions for a Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionSuggestions()
    {
        var query = new GetSuggestionsForStudentsQuery();
        ResponseDto<List<GetSupervisorSuggestion>> result = await this._sender.Send(query);
        return Ok(result);
    }
}