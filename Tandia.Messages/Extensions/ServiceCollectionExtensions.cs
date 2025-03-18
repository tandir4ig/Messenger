namespace Tandia.Messages.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tandia.Messages.Data;
using Tandia.Messages.Services;
using Tandia.Messages.Services.Interfaces;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(connectionString));

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IMessageService, MessageService>();

        return services;
    }
}
