using Dissertation.Application.SupervisorSuggestion.Commands.InitiateMatching;
using Dissertation.Application.SupervisorSuggestion.Commands.ProcessMatching;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;

namespace Dissertation_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized Request", typeof(ResponseDto<string>))]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
public class SupervisorSuggestionController : Controller
{
    private readonly ISender _sender;

    public SupervisorSuggestionController(ISender sender) => this._sender = sender;

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPost("initiate")]
    [SwaggerOperation(Summary = "Initiate the Matching Process")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> InitiateMatching()
    {
        var command = new InitiateMatchingCommand();
        ResponseDto<string> result = await this._sender.Send(command);
        return Ok(result);
    }

    [HttpPost("callback/{taskId}")]
    [SwaggerOperation(Summary = "Process the Matching Process")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> ProcessMatching([FromRoute] string taskId)
    {
        var command = new ProcessMatchingCommand(taskId);
        InitiateMatchingResponse result = await this._sender.Send(command);
        return Ok(result);
    }
}