using Dissertation.Application.Department.Commands.CreateDepartment;
using Dissertation.Application.Department.Commands.DisableDepartment;
using Dissertation.Application.Department.Commands.EnableDepartment;
using Dissertation.Application.Department.Commands.UpdateDepartment;
using Dissertation.Application.Department.Queries.GetAllDepartments;
using Dissertation.Application.Department.Queries.GetById;
using Dissertation.Application.Department.Queries.GetListOfActiveDepartment;
using Dissertation.Application.Department.Queries.GetListOfDepartment;
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
public class DepartmentController : Controller
{
    private readonly ISender _sender;

    public DepartmentController(ISender sender) => this._sender = sender;

    [HttpPost]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Create Department")]
    [SwaggerResponse(StatusCodes.Status201Created, "Request Successful", typeof(ResponseDto<GetDepartment>))]
    public async Task<IActionResult> CreateCourse([FromBody] CreateDepartmentRequest request)
    {
        var command = new CreateDepartmentCommand(request.Name);
        ResponseDto<GetDepartment> result = await this._sender.Send(command);
        return Created(result.Result?.Id.ToString()!, result);
    }

    [HttpPost("enable/{departmentId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Enable Department")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetDepartment>))]
    public async Task<IActionResult> EnableDepartment([FromRoute] long departmentId)
    {
        var command = new EnableDepartmentCommand(departmentId);
        ResponseDto<GetDepartment> result = await this._sender.Send(command);
        return Ok(result);
    }

    [HttpPost("disable/{departmentId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Disable Department")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetDepartment>))]
    public async Task<IActionResult> DisableDepartment([FromRoute] long departmentId)
    {
        var command = new DisableDepartmentCommand(departmentId);
        ResponseDto<GetDepartment> result = await this._sender.Send(command);
        return Ok(result);
    }

    [HttpGet("{departmentId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Get Department By Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetDepartment>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetDepartmentById([FromRoute] long departmentId)
    {
        var query = new GetDepartmentByIdQuery(departmentId);
        ResponseDto<GetDepartment> result = await this._sender.Send(query);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("active")]
    [SwaggerOperation(Summary = "Get List Of Active Departments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<IEnumerable<GetDepartment>>))]
    public async Task<IActionResult> GetListOfActiveDepartments()
    {
        var query = new GetListOfActiveDepartmentQuery();
        ResponseDto<IEnumerable<GetDepartment>> result = await this._sender.Send(query);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Get List of Department")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedDepartmentListDto>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetListOfDepartment([FromQuery] DepartmentPaginationParameters paginationParameters)
    {
        var query = new GetListOfDepartmentQuery(paginationParameters);
        ResponseDto<PaginatedDepartmentListDto> response = await this._sender.Send(query);
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

    [HttpPut("{departmentId:long}")]
    [Authorize(Roles = "Superadmin,Admin")]
    [SwaggerOperation(Summary = "Update Department")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetDepartment>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> UpdateDepartment([FromBody] CreateDepartmentRequest request, [FromRoute] long departmentId)
    {
        var query = new UpdateDepartmentCommand(request.Name, departmentId);
        ResponseDto<GetDepartment> response = await this._sender.Send(query);
        return Ok(response);
    }

    [HttpGet("all-departments")]
    [Authorize(Roles = "Superadmin,Admin,Supervisor, Student")]
    [SwaggerOperation(Summary = "Get All Departments")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<IReadOnlyList<GetDepartment>>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Not Found", typeof(CustomProblemDetails))]
    public async Task<IActionResult> GetAllDepartments()
    {
        var query = new GetAllDepartmentsQuery();
        ResponseDto<IReadOnlyList<GetDepartment>> response = await this._sender.Send(query);
        return Ok(response);
    }
}