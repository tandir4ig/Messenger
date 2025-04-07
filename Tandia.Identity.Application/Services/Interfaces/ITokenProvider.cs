namespace Tandia.Identity.Application.Services.Interfaces;

public interface ITokenProvider
{
    string GenerateAccessToken(Guid userId);

    string GenerateRefreshToken();
}
