using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.DTO;
using Shared.Helpers;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Extensions;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Controllers;
[Route("api/v{version:apiVersion}/[controller]")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
public class UserController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UserController(IAuthService authService, IUserService userService)
    {
        this._authService = authService;
        this._userService = userService;
    }

    [Authorize(Roles = "Superadmin")]
    [HttpPost("register-admin")]
    [SwaggerOperation(Summary = "Registration for admin users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegistrationRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<string> response = await this._authService.RegisterAdmin(model, email);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin")]
    [HttpPost("resend-confirm-email")]
    [SwaggerOperation(Summary = "Registration for admin users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] EmailRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<string> response = await this._authService.ResendConfirmationEmail(model, email);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin, Supervisor, Student")]
    [HttpGet("access-token")]
    [SwaggerOperation(Summary = "Get the details for a user using the access token")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetUserDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUser()
    {
        ResponseDto<GetUserDto> response = await this._userService.GetUser();
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin, Supervisor, Student")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get the details for a user using the user Id")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetUserDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        ResponseDto<GetUserDto> response = await this._userService.GetUser(id);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("get-by-email/{email}")]
    [SwaggerOperation(Summary = "Get the details for a user by email")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetUserDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
    {
        ResponseDto<GetUserDto> response = await this._userService.GetUserByEmail(email);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("get-by-username/{username}")]
    [SwaggerOperation(Summary = "Get the details for a user by username")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetUserDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserByUserName([FromRoute] string username)
    {
        ResponseDto<GetUserDto> response = await this._userService.GetUserByUserName(username);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPost("deactivate")]
    [SwaggerOperation(Summary = "Lock out or deactivate a user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<bool>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LockOutUser([FromBody] EmailRequestDto model)
    {
        var adminEmail = HttpContext.GetEmail();
        ResponseDto<bool> response = await this._userService.LockOutUser(model.Email, adminEmail);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpPost("activate")]
    [SwaggerOperation(Summary = "Activate or unlock a user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<bool>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlockUser([FromBody] EmailRequestDto model)
    {
        var adminEmail = HttpContext.GetEmail();
        ResponseDto<bool> response = await this._userService.UnlockUser(model.Email, adminEmail);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin")]
    [HttpGet("get-admin-users")]
    [SwaggerOperation(Summary = "List of Admin Users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PagedList<UserListDto>>))]
    public ActionResult GetAdminUsers([FromQuery] UserPaginationParameters paginationParameters)
    {
        var adminUserId = HttpContext.GetUserId();
        if (!string.IsNullOrEmpty(adminUserId))
            paginationParameters.LoggedInAdminId = adminUserId;

        ResponseDto<PaginatedUserListDto> users = this._userService.GetPaginatedAdminUsers(paginationParameters);
        return Ok(users);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("get-students")]
    [SwaggerOperation(Summary = "List of Students")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedUserListDto>))]
    public async Task<ActionResult> GetStudents([FromQuery] DissertationStudentPaginationParameters paginationParameters)
    {
        ResponseDto<PaginatedStudentListDto> users = await this._userService.GetPaginatedStudents(paginationParameters);
        return Ok(users);
    }

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("get-supervisors")]
    [SwaggerOperation(Summary = "List of Supervisors")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<PaginatedSupervisorListDto>))]
    public async Task<IActionResult> GetSupervisors([FromQuery] SupervisorPaginationParameters paginationParameters)
    {
        ResponseDto<PaginatedSupervisorListDto> users = await this._userService.GetPaginatedSupervisors(paginationParameters);
        return Ok(users);
    }


    [Authorize(Roles = "Superadmin")]
    [HttpPut("edit-admin-user")]
    [SwaggerOperation(Summary = "Edit an admin user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<EditUserRequestDto>))]
    public async Task<IActionResult> EditUser([FromBody] EditUserRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<EditUserRequestDto> response = await this._userService.EditUser(model, email);
        return Ok(response);
    }

    [HttpPost("register-supervisor")]
    [SwaggerOperation(Summary = "Register Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> RegisterSupervisor([FromBody] StudentOrSupervisorRegistrationDto model)
    {
        ResponseDto<string> response = await this._authService.RegisterSupervisor(model);
        return Ok(response);
    }

    [HttpPost("register-student")]
    [SwaggerOperation(Summary = "Register Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> RegisterStudent([FromBody] StudentOrSupervisorRegistrationDto model)
    {
        ResponseDto<string> response = await this._authService.RegisterStudent(model);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin, Supervisor, Student")]
    [HttpPost("edit-student")]
    [SwaggerOperation(Summary = "Edit a Student")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> EditStudent([FromBody] EditStudentRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<UserDto> response = await this._userService.EditStudent(model, email);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin, Admin, Supervisor, Student")]
    [HttpPost("edit-supervisor")]
    [SwaggerOperation(Summary = "Edit a Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> EditSupervisor([FromBody] EditSupervisorRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<UserDto> response = await this._userService.EditSupervisor(model, email);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin")]
    [HttpPost("assign-supervisor-role")]
    [SwaggerOperation(Summary = "Assign Supervisor role to admin")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> AssignSupervisorRoleToAdmin([FromBody] AssignSupervisorRoleRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<UserDto> response = await this._authService.AssignSupervisorRoleToAdmin(model, email);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin")]
    [HttpPost("change-admin-role")]
    [SwaggerOperation(Summary = "Change Admin Role")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> ChangeAdminRole([FromBody] EmailRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<UserDto> response = await this._authService.ChangeAdminRole(model, email);
        return Ok(response);
    }

    [Authorize(Roles = "Superadmin")]
    [HttpPost("assign-admin-role")]
    [SwaggerOperation(Summary = "Assign Admin Role to a Supervisor")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<UserDto>))]
    public async Task<IActionResult> AssignAdminRoleToSupervisor([FromBody] AssignAdminRoleRequestDto model)
    {
        var email = HttpContext.GetEmail();
        ResponseDto<UserDto> response = await this._authService.AssignAdminRoleToSupervisor(model, email);
        return Ok(response);
    }
}