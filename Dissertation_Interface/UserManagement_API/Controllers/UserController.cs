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

    [Authorize(Roles = "Superadmin, Admin")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get the details for a user")]
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

        if (users.Result != null)
        {
            var metadata = new
            {
                users.Result.TotalCount,
                users.Result.PageSize,
                users.Result.CurrentPage,
                users.Result.TotalPages,
                users.Result.HasNext,
                users.Result.HasPrevious
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        }

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
}