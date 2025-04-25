using CSharpFunctionalExtensions;
using Dapper;
using Npgsql;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(string connectionString) : IRefreshTokenRepository
{
    public async Task<Result> AddAsync(RefreshTokenEntity refreshToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);

            await connection.ExecuteAsync(
                "INSERT INTO \"RefreshTokens\" (\"Id\", \"UserId\", \"Token\", \"ExpiryDate\", \"IsValid\") VALUES (@Id, @UserId, @Token, @ExpiryDate, @IsValid)",
                new { refreshToken.Id, refreshToken.UserId, refreshToken.Token, refreshToken.ExpiryDate, refreshToken.IsValid });

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to add refresh token: {ex.Message}");
        }
    }

    public async Task<Result<RefreshTokenEntity>> GetTokenAsync(string refreshToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);

            var result = await connection.QueryFirstOrDefaultAsync(
                "SELECT \"Id\", \"UserId\", \"Token\", \"ExpiryDate\", \"IsValid\" " +
                "FROM \"RefreshTokens\" " +
                "WHERE \"Token\" = @Token " +
                "ORDER BY \"ExpiryDate\" DESC " +
                "LIMIT 1",
                new { Token = refreshToken });

            if (result == null)
            {
                return Result.Failure<RefreshTokenEntity>("Token not found.");
            }

            var refreshTokenEntity = new RefreshTokenEntity(
                result.Id,
                result.UserId,
                result.Token,
                result.ExpiryDate,
                result.IsValid);

            return Result.Success(refreshTokenEntity);
        }
        catch (Exception ex)
        {
            return Result.Failure<RefreshTokenEntity>($"Failed to retrieve token: {ex.Message}");
        }
    }

    public async Task<Result> InvalidateTokenAsync(string refreshToken)
    {
        try
        {
            await using var connection = new NpgsqlConnection(connectionString);

            await connection.ExecuteAsync(
                "UPDATE \"RefreshTokens\" SET \"IsValid\" = FALSE WHERE \"Token\" = @Token",
                new { Token = refreshToken });

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to invalidate token: {ex.Message}");
        }
    }
}
