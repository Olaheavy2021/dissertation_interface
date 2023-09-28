using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Controllers;
[Route("api/v{version:apiVersion}/superadmin")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
[Authorize(Roles = "Superadmin")]
public class SuperAdminController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public SuperAdminController(IAuthService authService, IUserService userService)
    {
        this._authService = authService;
        this._userService = userService;
    }

    [HttpPost("register-admin")]
    [SwaggerOperation(Summary = "Registration for admin users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<RegistrationRequestDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegistrationRequestDto model)
    {
        ResponseDto<RegistrationRequestDto> response = await this._authService.RegisterAdmin(model);
        return Ok(response);
    }

    [HttpPost("resend-confirm-email")]
    [SwaggerOperation(Summary = "Registration for admin users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<string>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ResendConfirmationEmail([FromBody] EmailRequestDto model)
    {
        ResponseDto<string> response = await this._authService.ResendConfirmationEmail(model);
        return Ok(response);
    }

    [HttpGet("admin-users")]
    [SwaggerOperation(Summary = "Get all admin users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<List<UserDto>>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAdminUsers()
    {
        ResponseDto<List<UserDto>> response = await this._userService.GetAdminUsers();
        return Ok(response);
    }

    [HttpGet("admin-user/{id}")]
    [SwaggerOperation(Summary = "Get an admin user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<GetUserDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAdminUser(string id)
    {
        ResponseDto<GetUserDto> response = await this._userService.GetUser(id);
        return Ok(response);
    }

    [HttpPost("user/deactivate")]
    [SwaggerOperation(Summary = "Lock out or deactivate a user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<bool>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LockOutUser(string email)
    {
        ResponseDto<bool> response = await this._userService.LockOutUser(email);
        return Ok(response);
    }

    [HttpPost("user/activate")]
    [SwaggerOperation(Summary = "Activate or unlock a user")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<bool>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UnlockUser(string email)
    {
        ResponseDto<bool> response = await this._userService.UnlockUser(email);
        return Ok(response);
    }


}