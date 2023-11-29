using Dissertation.Application.DTO.Request;
using Dissertation.Application.Student.Commands.RegisterStudent;
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

}