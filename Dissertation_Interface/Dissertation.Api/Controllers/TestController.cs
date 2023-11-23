using Dissertation.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;

namespace Dissertation_API.Controllers;

[Authorize(Roles = "Superadmin,Admin")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class TestController : Controller
{
    private readonly IUserApiService _userApiService;

    public TestController(IUserApiService userApiService) => this._userApiService = userApiService;

    [HttpGet("get-by-email/{email}")]
    public async Task<IActionResult> GetUserByEmail([FromRoute] string email)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByEmail(email);
        return Ok(response);
    }

    [HttpGet("get-by-username/{username}")]
    public async Task<IActionResult> GetUserByUsername([FromRoute] string username)
    {
        ResponseDto<GetUserDto> response = await this._userApiService.GetUserByUserName(username);
        return Ok(response);
    }
}