using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public sealed class RefreshTokenRepository(IOptions<DatabaseOptions> databaseOptions) : IRefreshTokenRepository
{
    private readonly string connectionString = databaseOptions.Value.ConnectionString;

    public async Task AddAsync(RefreshTokenEntity refreshToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
        INSERT INTO "RefreshTokens"
               ("Id", "UserId", "Token", "ExpiryDate")
        VALUES  (@Id, @UserId, @Token, @ExpiryDate);
        """;

        await connection.ExecuteAsync(Sql, new
        {
            refreshToken.Id,
            refreshToken.UserId,
            refreshToken.Token,
            refreshToken.ExpiryDate,
        });
    }

    public async Task<RefreshTokenEntity?> GetTokenAsync(string refreshToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
        SELECT  "Id",
                "UserId",
                "Token",
                "ExpiryDate"
        FROM    "RefreshTokens"
        WHERE   "Token" = @Token
        ORDER BY "ExpiryDate" DESC
        LIMIT 1;
        """;

        var result = await connection.QueryFirstOrDefaultAsync(Sql, new { Token = refreshToken });

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

    public async Task DeleteAsync(string refreshToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
        DELETE FROM "RefreshTokens"
        WHERE  "Token" = @Token;
        """;

        await connection.ExecuteAsync(Sql, new { Token = refreshToken });
    }
}
