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
        ResponseDto loginResponse = await this._authService.Login(model);
        if (loginResponse.IsSuccess)
        {
            return Ok(loginResponse);
        }

        return Unauthorized(loginResponse);
    }
}