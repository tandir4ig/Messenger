using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tandia.Messages.Application.Consumers;
using Tandia.Messages.Application.Models;
using Tandia.Messages.Application.Services;
using Tandia.Messages.Application.Services.Interfaces;

namespace Tandia.Messages.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessages(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IMessageService, MessageService>();

        return services;
    }

    public static IServiceCollection AddRabbitMqMessages(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UserLoggedInConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(options.Host, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
                // Определяем очередь (endpoint) для получения событий UserLoggedIn
                cfg.ReceiveEndpoint(options.QueueName, e =>
                {
                    e.ConfigureConsumer<UserLoggedInConsumer>(context);
                });
            });
        });

        return services;
    }
}
