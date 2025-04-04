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
}
