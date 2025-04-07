using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tandia.Identity.Application.Enums;
using Tandia.Identity.Application.Services.Interfaces;
using Tandia.Identity.WebApi.DTOs.Requests;

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
            return Ok(loginResponse);
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

    [Authorize]
    [HttpGet("test")]
    public ActionResult Test()
    {
        return Ok();
    }
}
