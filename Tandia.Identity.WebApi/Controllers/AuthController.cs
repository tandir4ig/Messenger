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
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Email and password are required.");
        }

        try
        {
            var loginResponse = await identityService.LoginUserAsync(request.Email, request.Password);
            var loginResponseDto = new LoginResponse(loginResponse.AccessToken, loginResponse.RefreshToken);
            return Ok(loginResponseDto);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(); // Возвращает 401 Unauthorized
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest("Refresh token is required.");
        }

        try
        {
            var loginResponse = await identityService.RefreshTokenAsync(request.RefreshToken);
            return Ok(new LoginResponse(loginResponse.AccessToken, loginResponse.RefreshToken));
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(); // Возвращает 401 Unauthorized, если refresh token недействителен
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpGet("test")]
    public ActionResult Test()
    {
        return Ok();
    }
}
