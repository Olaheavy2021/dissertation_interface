using Dissertation.Application.DTO.Request;
using Dissertation.Application.Supervisor.Commands.RegisterSupervisor;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;

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

}