using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Helpers;
using Shared.Middleware;
using Swashbuckle.AspNetCore.Annotations;
using UserManagement_API.Data.Models.Dto;
using UserManagement_API.Service.IService;

namespace UserManagement_API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(CustomProblemDetails))]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => this._authService = authService;


    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login for all users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Successful", typeof(ResponseDto<AuthResponseDto>))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Request Unsuccessful", typeof(ResponseDto<string>))]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        ResponseDto<AuthResponseDto> response = await this._authService.Login(model);
        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return Unauthorized(response);
    }

    [HttpPost("initiate-reset-password")]
    [SwaggerOperation(Summary = "Initiate reset password for all users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Processed", typeof(ResponseDto<string>))]
    public async Task<IActionResult> InitiateResetPassword([FromBody] InitiatePasswordResetDto model)
    {
        ResponseDto<string> response = await this._authService.InitiatePasswordReset(model);
        return Ok(response);
    }

    [HttpPost("confirm-reset-password")]
    [SwaggerOperation(Summary = "Confirm reset password for all users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Processed", typeof(ResponseDto<string>))]
    public async Task<IActionResult> ConfirmResetPassword([FromBody] ConfirmPasswordResetDto model)
    {
        ResponseDto<string> response = await this._authService.ConfirmPasswordReset(model);
        return Ok(response);
    }

    [HttpPost("confirm-email")]
    [SwaggerOperation(Summary = "Confirm email for all users")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Processed", typeof(ResponseDto<ConfirmEmailResponseDto>))]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestDto model)
    {
        ResponseDto<ConfirmEmailResponseDto> response = await this._authService.ConfirmEmail(model);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(Summary = "Refresh Token")]
    [SwaggerResponse(StatusCodes.Status200OK, "Request Processed", typeof(ResponseDto<AuthResponseDto>))]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
    {
        ResponseDto<RefreshTokenDto> response = await this._authService.GetRefreshToken(model);
        return Ok(response);
    }
}