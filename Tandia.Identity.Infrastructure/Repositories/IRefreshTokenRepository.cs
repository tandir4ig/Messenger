using CSharpFunctionalExtensions;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public interface IRefreshTokenRepository
{
    Task<Result> AddAsync(RefreshTokenEntity refreshToken);

    Task<Result<RefreshTokenEntity>> GetTokenAsync(string refreshToken);

    Task<Result> InvalidateTokenAsync(string refreshToken);
}
