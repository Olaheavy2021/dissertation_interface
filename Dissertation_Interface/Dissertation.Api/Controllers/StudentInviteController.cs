using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Application.StudentInvite.Commands.ConfirmStudentInvite;
using Dissertation.Application.StudentInvite.Commands.CreateStudentInvite;
using Dissertation.Application.StudentInvite.Commands.DeleteStudentInvite;
using Dissertation.Application.StudentInvite.Commands.UpdateStudentInvite;
using Dissertation.Application.StudentInvite.Queries.GetById;
using Dissertation.Application.StudentInvite.Queries.GetListOfStudentInviteQueryHandler;
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
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Request Unsuccessful", typeof(ResponseDto<string>))]
public class StudentInviteController : Controller
{
    private readonly ISender _sender;

    public StudentInviteController(ISender sender) => this._sender = sender;

    [HttpPost]
    [SwaggerOperation(Summary = "Create Student Invite")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<ResponseDto<GetStudentInvite>>))]
    public async Task<IActionResult> CreateStudentInvite([FromBody] CreateStudentInviteRequest request)
    {
        var command = new CreateStudentInviteCommand(request.LastName, request.FirstName, request.StudentId, request.Email);
        ResponseDto<GetStudentInvite> result = await this._sender.Send(command);
        return Created(result.Result?.Id.ToString()!, result);
    }

    [HttpGet("{inviteId:long}")]
    [SwaggerOperation(Summary = "Get Student Invite By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetStudentInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetStudentInviteById([FromRoute] long inviteId)
    {
        var query = new GetStudentInviteByIdQuery(inviteId);
        ResponseDto<GetStudentInvite> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpPut("{inviteId:long}")]
    [SwaggerOperation(Summary = "Update Student Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetStudentInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> UpdateStudentInvite([FromBody] CreateStudentInviteRequest request, [FromRoute] long inviteId)
    {
        var command = new UpdateStudentInviteCommand(request.LastName, request.FirstName, request.StudentId, request.Email, request.CourseId, inviteId);
        ResponseDto<GetStudentInvite> response = await this._sender.Send(command);
        return Ok(response);
    }

    [HttpDelete("{inviteId:long}")]
    [SwaggerOperation(Summary = "Delete Student Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetStudentInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> DeleteStudentInvite([FromRoute] long inviteId)
    {
        var command = new DeleteStudentInviteCommand(inviteId);
        ResponseDto<GetStudentInvite> response = await this._sender.Send(command);
        return Ok(response);
    }

    [HttpPost("confirm-invite")]
    [SwaggerOperation(Summary = "Confirm Student Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetStudentInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> ConfirmStudentInvite([FromBody] ConfirmStudentInviteRequest request)
    {
        var command = new ConfirmStudentInviteCommand(request.StudentId, request.InvitationCode);
        ResponseDto<GetStudentInvite> response = await this._sender.Send(command);
        return Ok(response);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Student Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedStudentInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfStudentInvite([FromQuery] StudentInvitePaginationParameters paginationParameters)
    {
        var query = new GetStudentInviteListQuery(paginationParameters);
        ResponseDto<PaginatedStudentInvite> response = await this._sender.Send(query);

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
}