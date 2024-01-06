using Dissertation.Application.DTO.Request;
using Dissertation.Application.DTO.Response;
using Dissertation.Application.StudentInvite.Commands.ResendStudentInvite;
using Dissertation.Application.SupervisorInvite.Commands.ConfirmSupervisorInvite;
using Dissertation.Application.SupervisorInvite.Commands.CreateSupervisorInvite;
using Dissertation.Application.SupervisorInvite.Commands.DeleteSupervisorInvite;
using Dissertation.Application.SupervisorInvite.Commands.ResendSupervisorInvite;
using Dissertation.Application.SupervisorInvite.Commands.UpdateSupervisorInvite;
using Dissertation.Application.SupervisorInvite.Commands.UploadSupervisorInvite;
using Dissertation.Application.SupervisorInvite.Queries.GetById;
using Dissertation.Application.SupervisorInvite.Queries.GetListOfSupervisorInvite;
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
public class SupervisorInviteController : Controller
{
    private readonly ISender _sender;

    public SupervisorInviteController(ISender sender) => this._sender = sender;

    [HttpPost]
    [SwaggerOperation(Summary = "Create Supervisor Invite")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<ResponseDto<GetSupervisorInvite>>))]
    public async Task<IActionResult> CreateSupervisorInvite([FromBody] CreateSupervisorInviteRequest request)
    {
        var command = new CreateSupervisorInviteCommand(request.LastName, request.FirstName, request.StaffId, request.Email);
        ResponseDto<GetSupervisorInvite> result = await this._sender.Send(command);
        return Created(result.Result?.Id.ToString()!, result);
    }

    [HttpGet("{inviteId:long}")]
    [SwaggerOperation(Summary = "Get Supervision Invite By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetSupervisorInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetSupervisionInviteById([FromRoute] long inviteId)
    {
        var query = new GetSupervisionInviteByIdQuery(inviteId);
        ResponseDto<GetSupervisorInvite> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpPut("{inviteId:long}")]
    [SwaggerOperation(Summary = "Update Supervisor Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetSupervisorInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> UpdateSupervisorInvite([FromBody] CreateSupervisorInviteRequest request, [FromRoute] long inviteId)
    {
        var command = new UpdateSupervisorInviteCommand(request.LastName, request.FirstName, request.StaffId, request.Email, inviteId);
        ResponseDto<GetSupervisorInvite> response = await this._sender.Send(command);
        return Ok(response);
    }

    [HttpDelete("{inviteId:long}")]
    [SwaggerOperation(Summary = "Delete Supervisor Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetSupervisorInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> DeleteSupervisorInvite([FromRoute] long inviteId)
    {
        var command = new DeleteSupervisorInviteCommand(inviteId);
        ResponseDto<GetSupervisorInvite> response = await this._sender.Send(command);
        return Ok(response);
    }


    [AllowAnonymous]
    [HttpPost("confirm-invite")]
    [SwaggerOperation(Summary = "Confirm Supervisor Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetSupervisorInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> ConfirmSupervisorInvite([FromBody] ConfirmSupervisorInviteRequest request)
    {
        var command = new ConfirmSupervisorInviteCommand(request.StaffId, request.InvitationCode);
        ResponseDto<GetSupervisorInvite> response = await this._sender.Send(command);
        return Ok(response);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get List of Supervision Invite")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedSupervisorInvite>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfSupervisionInvite([FromQuery] SupervisorInvitePaginationParameters paginationParameters)
    {
        var query = new GetSupervisorInviteListQuery(paginationParameters);
        ResponseDto<PaginatedSupervisorInvite> response = await this._sender.Send(query);

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

    [HttpPost("upload")]
    [SwaggerOperation(Summary = "Upload Supervisor Invites")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<ResponseDto<string>>))]
    public async Task<IActionResult> UploadSupervisorInvite([FromBody] UploadInvitesRequest request)
    {
        var command = new UploadSupervisorInviteCommand(request.Requests);
        ResponseDto<string> response = await this._sender.Send(command);
        return Ok(response);
    }

    [HttpPost("resend-invite/{inviteId:long}")]
    [SwaggerOperation(Summary = "Resend Supervisor Invite")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<GetSupervisorInvite>))]
    public async Task<IActionResult> ResendSupervisorInvite([FromRoute] long inviteId)
    {
        var command = new ResendSupervisorInviteCommand(inviteId);
        ResponseDto<GetSupervisorInvite> response = await this._sender.Send(command);
        return Ok(response);
    }
}