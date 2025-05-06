using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public sealed class UserRepository(IOptions<DatabaseOptions> databaseOptions) : IUserRepository
{
    private readonly string connectionString = databaseOptions.Value.ConnectionString;

    public async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            SELECT "Id", "RegistrationDate"
            FROM   "Users"
            WHERE  "Id" = @Id;
            """;

        return await connection.QuerySingleOrDefaultAsync<UserEntity>(Sql, new { Id = id });
    }

    public async Task<IReadOnlyCollection<UserEntity>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            SELECT "Id", "RegistrationDate"
            FROM   "Users";
            """;

        var list = await connection.QueryAsync<UserEntity>(Sql);

        return list.ToList().AsReadOnly();
    }

    public async Task AddAsync(UserEntity user)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            INSERT INTO "Users" ("Id", "RegistrationDate")
            VALUES (@Id, @RegistrationDate);
            """;

        await connection.ExecuteAsync(Sql, user);
    }

    public async Task UpdateAsync(UserEntity user)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            UPDATE "Users"
            SET    "RegistrationDate" = @RegistrationDate
            WHERE  "Id"             = @Id;
            """;

        await connection.ExecuteAsync(Sql, user);
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            DELETE FROM "Users"
            WHERE  "Id" = @Id;
            """;

        await connection.ExecuteAsync(Sql, new { Id = id });
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        const string Sql = """
            SELECT "Id", "RegistrationDate"
            FROM   "Users"
            WHERE  "Email" = @Email;
            """;

        return await connection.QuerySingleOrDefaultAsync<UserEntity>(Sql, new { Email = email });
    }
}
