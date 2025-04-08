using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshTokenEntity refreshToken);

    Task<RefreshTokenEntity?> GetTokenAsync(string refreshToken);
}
