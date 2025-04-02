using Dapper;
using Npgsql;
using Tandia.Identity.Application.Repositories;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Repositories;

public class UserRepository(string connectionString) : IRepository<UserEntity>
{
    private readonly string _connectionString = connectionString;

    public async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<UserEntity>(
            "SELECT * FROM \"Users\" WHERE \"Id\" = @Id",
            new { Id = id });
    }

    public async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        return await connection.QueryAsync<UserEntity>(
            "SELECT * FROM \"Users\"");
    }

    public async Task AddAsync(UserEntity user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "INSERT INTO \"Users\" (\"Id\", \"RegistrationDate\") VALUES (@Id, @RegistrationDate)",
            user);
    }

    public async Task UpdateAsync(UserEntity user)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "UPDATE \"Users\" SET \"RegistrationDate\" = @RegistrationDate WHERE \"Id\" = @Id",
            user);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync(
            "DELETE FROM \"Users\" WHERE \"Id\" = @Id",
            new { Id = id });
    }
}
