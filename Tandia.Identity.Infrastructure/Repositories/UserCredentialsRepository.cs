using Dapper;
using Npgsql;
using Tandia.Identity.Application.Repositories;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public class UserCredentialsRepository(string connectString) : IRepository<UserCredentialsEntity>
{
    private readonly string _connectionString = connectString;

    public async Task<UserCredentialsEntity?> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<UserCredentialsEntity>(
            "SELECT * FROM \"UserCredentials\" WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<UserCredentialsEntity>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QueryAsync<UserCredentialsEntity>(
            "SELECT * FROM \"UserCredentials\"");
    }

    public async Task AddAsync(UserCredentialsEntity userCredentials)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO \"UserCredentials\" (\"Id\", \"Email\", \"PasswordHash\", \"Salt\") VALUES (@Id, @Email, @PasswordHash, @Salt)",
            userCredentials);
    }

    public async Task UpdateAsync(UserCredentialsEntity userCredentials)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "UPDATE \"UserCredentials\" SET \"Email\" = @Email, \"PasswordHash\" = @PasswordHash, \"Salt\" = @Salt WHERE \"Id\" = @Id",
            userCredentials);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "DELETE FROM \"UserCredentials\" WHERE \"Id\" = @Id",
            new { Id = id });
    }
}
