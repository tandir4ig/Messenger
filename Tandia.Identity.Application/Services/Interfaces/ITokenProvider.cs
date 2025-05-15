namespace Tandia.Identity.WebApi.Services.Interfaces;

public interface ITokenProvider
{
    string GenerateAccessToken(Guid userId);

    string GenerateRefreshToken();
}
