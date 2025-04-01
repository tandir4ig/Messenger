using Tandia.Messages.Application.Extensions;
using Tandia.Messages.Infrastructure.Extensions;

namespace Tandia.Messages.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageServices(this IServiceCollection services, string connectionString)
    {
        services.AddMessages();
        services.AddInfrastructure(connectionString);

        return services;
    }
}
