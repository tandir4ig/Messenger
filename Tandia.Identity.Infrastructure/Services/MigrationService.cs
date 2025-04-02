using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Tandia.Identity.Infrastructure.Migrations;

namespace Tandia.Identity.Infrastructure.Services;

public sealed class MigrationService(IConfiguration configuration) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(configuration.GetConnectionString(
            "DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."));

        await connection.OpenAsync(cancellationToken);
        await connection.ExecuteAsync(MigrationScripts.InitialMigration);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
