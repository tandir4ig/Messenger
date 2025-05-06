using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public sealed class UserCredentialsRepository(IOptions<DatabaseOptions> databaseOptions) : IUserCredentialsRepository
{
    private readonly string connectionString = databaseOptions.Value.ConnectionString;

    public async Task<UserCredentialsEntity?> GetByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            SELECT  "Id", "Email", "PasswordHash", "Salt"
            FROM    "UserCredentials"
            WHERE   "Id" = @Id;
            """;

        return await connection.QuerySingleOrDefaultAsync<UserCredentialsEntity>(Sql, new { Id = id });
    }

    public async Task<UserCredentialsEntity?> GetByEmailAsync(string email)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            SELECT  "Id", "Email", "PasswordHash", "Salt"
            FROM    "UserCredentials"
            WHERE   "Email" = @Email;
            """;

        return await connection.QuerySingleOrDefaultAsync<UserCredentialsEntity>(Sql, new { Email = email });
    }

    public async Task<IReadOnlyCollection<UserCredentialsEntity>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            SELECT  "Id", "Email", "PasswordHash", "Salt"
            FROM    "UserCredentials";
            """;

        var result = await connection.QueryAsync<UserCredentialsEntity>(Sql);

        return result.ToList().AsReadOnly();
    }

    public async Task AddAsync(UserCredentialsEntity userCredentials)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            INSERT INTO "UserCredentials" ("Id", "Email", "PasswordHash", "Salt")
            VALUES (@Id, @Email, @PasswordHash, @Salt);
            """;

        await connection.ExecuteAsync(Sql, new
        {
            userCredentials.Id,
            userCredentials.Email,
            userCredentials.PasswordHash,
            userCredentials.Salt,
        });
    }

    public async Task UpdateAsync(UserCredentialsEntity userCredentials)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            UPDATE "UserCredentials"
            SET    "Email"        = @Email,
                   "PasswordHash" = @PasswordHash,
                   "Salt"         = @Salt
            WHERE  "Id"           = @Id;
            """;

        await connection.ExecuteAsync(Sql, userCredentials);
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            DELETE FROM "UserCredentials"
            WHERE "Id" = @Id;
            """;

        await connection.ExecuteAsync(Sql, new { Id = id });
    }
}
