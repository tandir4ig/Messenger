using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Tandia.Identity.Application.Models;
using Tandia.Identity.Application.Services.Interfaces;

namespace Tandia.Identity.Application.Services;

public sealed class TokenProvider(
    IOptions<JwtSettings> jwtSettings,
    TimeProvider timeProvider)
    : ITokenProvider
{
    public string GenerateAccessToken(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.SecretKey));

        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Value.Issuer,
            audience: jwtSettings.Value.Audience,
            claims: claims,
            expires: timeProvider.GetUtcNow().AddMinutes(jwtSettings.Value.TokenLifetime).DateTime,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}
