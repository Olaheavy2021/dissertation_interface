using Dissertation.Application.DTO.Request;
using Dissertation.Application.SupervisionRequest.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;

namespace Dissertation_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]"), ApiVersion("1.0"),
 SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails)),
 SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized Request", typeof(ResponseDto<string>))]
public class SupervisionRequestController : Controller
{
    private readonly ISender _sender;

    public SupervisionRequestController(ISender sender) => this._sender = sender;

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet]
    [SwaggerOperation(Summary = "Get Supervision Requests")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetSupervisionRequests([FromQuery] AdminSupervisionRequestParameters parameters)
    {
        var query = new GetSupervisionRequestListQuery(parameters);
        ResponseDto<PaginatedSupervisionRequestListDto> result = await this._sender.Send(query);
        return Ok(result);
    }

}