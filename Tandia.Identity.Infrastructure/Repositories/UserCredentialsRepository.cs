using Dapper;
using Npgsql;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public sealed class UserCredentialsRepository(string connectString) : IRepository<UserCredentialsEntity>
{
    private readonly string _connectionString = connectString;

    public async Task<UserCredentialsEntity?> GetByIdAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<UserCredentialsEntity>(
            "SELECT * FROM \"UserCredentials\" WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task<UserCredentialsEntity?> GetByEmailAsync(string email)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        return await connection.QuerySingleOrDefaultAsync<UserCredentialsEntity>(
            "SELECT * FROM \"UserCredentials\" WHERE \"Email\" = @Email",
            new { Email = email });
    }

    public async Task<IReadOnlyCollection<UserCredentialsEntity>> GetAllAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var result = await connection.QueryAsync<UserCredentialsEntity>(
            "SELECT * FROM \"UserCredentials\"");

        return result.ToList().AsReadOnly();
    }

    public async Task AddAsync(UserCredentialsEntity userCredentials)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "INSERT INTO \"UserCredentials\" (\"Id\", \"Email\", \"PasswordHash\", \"Salt\") " +
            "VALUES (@Id, @Email, @PasswordHash, @Salt)",
            userCredentials);
    }

    public async Task UpdateAsync(UserCredentialsEntity userCredentials)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "UPDATE \"UserCredentials\" " +
            "SET \"Email\" = @Email, \"PasswordHash\" = @PasswordHash, \"Salt\" = @Salt, \"RefreshToken\" = @RefreshToken  " +
            "WHERE \"Id\" = @Id",
            userCredentials);
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.ExecuteAsync(
            "DELETE FROM \"UserCredentials\" " +
            "WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task UpdateRefreshTokenAsync(Guid id, string refreshToken)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE \"UserCredentials\" " +
            "SET \"RefreshToken\" = @RefreshToken " +
            "WHERE \"Id\" = @Id",
            new { Id = id, RefreshToken = refreshToken });
    }
}
