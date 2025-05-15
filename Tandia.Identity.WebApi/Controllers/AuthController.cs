using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tandia.Identity.WebApi.DTOs.Requests;
using Tandia.Identity.WebApi.Services.Interfaces;

namespace Tandia.Identity.WebApi.Controllers;

[Route("api/auth")]
[ApiController]
public sealed class AuthController(IIdentityService identityService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await identityService.RegisterUserAsync(request.Email, request.Password);

        if (result.IsFailure)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await identityService.LoginUserAsync(request.Email, request.Password);

        if (result.IsFailure)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(result.Value);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await identityService.RefreshTokenAsync(request.RefreshToken);

        if (result.IsFailure)
        {
            return Problem(detail: result.Error, statusCode: StatusCodes.Status400BadRequest);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("test")]
    public ActionResult Test()
    {
        return Ok();
    }
}
