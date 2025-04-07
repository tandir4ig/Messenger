namespace Tandia.Identity.Application.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId);

    string GenerateRefreshToken();
}
