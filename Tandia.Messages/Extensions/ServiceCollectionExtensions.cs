namespace Tandia.Messages.Application.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tandia.Messages.Application.Services;
using Tandia.Messages.Application.Services.Interfaces;
using Tandia.Messages.Infrastructure.Data;
using Tandia.Messages.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessages(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionString));

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IMessageService, MessageService>();

        services.AddHostedService<DatabaseMigrationService>();

        return services;
    }
}
