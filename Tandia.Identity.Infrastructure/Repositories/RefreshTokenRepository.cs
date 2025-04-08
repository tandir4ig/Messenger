using Dapper;
using Npgsql;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(string connectionString) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshTokenEntity refreshToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        await connection.ExecuteAsync(
            "INSERT INTO \"RefreshTokens\" (\"Id\", \"UserId\", \"Token\", \"ExpiryDate\") VALUES (@Id, @UserId, @Token, @ExpiryDate)",
            refreshToken);
    }

    public async Task<RefreshTokenEntity?> GetTokenAsync(string refreshToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        var result = await connection.QueryFirstOrDefaultAsync(
            "SELECT \"Id\", \"UserId\", \"Token\", \"ExpiryDate\" FROM \"RefreshTokens\" " +
            "WHERE \"Token\" = @Token " +
            "ORDER BY \"ExpiryDate\" DESC " +
            "LIMIT 1",
            new { Token = refreshToken });

        if (result == null)
        {
            return null;
        }

        return new RefreshTokenEntity(
            result.Id,
            result.UserId,
            result.Token,
            result.ExpiryDate);
    }
}
