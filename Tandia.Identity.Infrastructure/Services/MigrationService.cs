using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Npgsql;
using Tandia.Identity.Infrastructure.Migrations;
using Tandia.Identity.Infrastructure.Models;

namespace Tandia.Identity.Infrastructure.Services;

public sealed class MigrationService(IOptions<DatabaseOptions> databaseOptions) : IHostedService
{
    private readonly string connectionString = databaseOptions.Value.ConnectionString;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(MigrationScripts.InitialMigration);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
