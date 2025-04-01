namespace Tandia.Messages.Application.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Tandia.Messages.Application.Services;
using Tandia.Messages.Application.Services.Interfaces;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessages(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IMessageService, MessageService>();

        return services;
    }
}
