using Microsoft.AspNetCore.Mvc;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Controllers;

[Route("api/auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        this._authService = authService;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        ResponseDto response = await this._authService.Login(model);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return Unauthorized(response);
    }

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegistrationRequestDto model)
    {
        ResponseDto response = await this._authService.RegisterAdmin(model);
        return Ok(response);
    }

    [HttpPost("initiate-reset-password")]
    public async Task<IActionResult> InitiateResetPassword([FromBody] InitiatePasswordResetDto model)
    {
        ResponseDto response = await this._authService.InitiatePasswordReset(model);
        return Ok(response);
    }

    [HttpPost("confirm-reset-password")]
    public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmPasswordResetDto model)
    {
        ResponseDto response = await this._authService.ConfirmPasswordReset(model);
        return Ok(response);
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto model)
    {
        ResponseDto response = await this._authService.ConfirmEmail(model);
        return Ok(response);
    }
}