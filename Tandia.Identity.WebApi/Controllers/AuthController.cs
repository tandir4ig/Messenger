using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tandia.Identity.Application.Enums;
using Tandia.Identity.Application.Services.Interfaces;
using Tandia.Identity.WebApi.DTOs.Requests;
using Tandia.Identity.WebApi.DTOs.Responses;

namespace Tandia.Identity.WebApi.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await identityService.RegisterUserAsync(request.Email, request.Password);

        return result switch
        {
            UserStatus.Registered=> Created(),

            _ => BadRequest(),
        };
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var loginResponse = await identityService.LoginUserAsync(request.Email, request.Password);
        var loginResponseDto = new LoginResponse(loginResponse.AccessToken, loginResponse.RefreshToken);
        return Ok(loginResponseDto);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var loginResponse = await identityService.RefreshTokenAsync(request.RefreshToken);
        return Ok(new LoginResponse(loginResponse.AccessToken, loginResponse.RefreshToken));
    }

    [Authorize]
    [HttpGet("test")]
    public ActionResult Test()
    {
        return Ok();
    }
}
