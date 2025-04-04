using Dapper;
using Npgsql;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public sealed class UserRepository(string connectionString) : IRepository<UserEntity>
{
    private readonly string _connectionString = connectionString;

    public async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<UserEntity>(
            "SELECT * FROM \"Users\" WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task<IReadOnlyCollection<UserEntity>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var result = await connection.QueryAsync<UserEntity>(
            "SELECT * FROM \"Users\"");

        return result.ToList().AsReadOnly();
    }

    public async Task AddAsync(UserEntity user)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "INSERT INTO \"Users\" (\"Id\", \"RegistrationDate\") VALUES (@Id, @RegistrationDate)",
            user);
    }

    public async Task UpdateAsync(UserEntity user)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "UPDATE \"Users\" SET \"RegistrationDate\" = @RegistrationDate WHERE \"Id\" = @Id",
            user);
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "DELETE FROM \"Users\" WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task<UserEntity?> GetByEmailAsync(string email)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<UserEntity>(
            "SELECT * FROM \"Users\" WHERE \"Email\" = @Email",
            new { Email = email });
    }
}
