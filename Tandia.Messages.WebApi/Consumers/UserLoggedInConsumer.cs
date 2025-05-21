using Contracts.Events;
using MassTransit;
using Tandia.Messages.Application.Services.Interfaces;

namespace Tandia.Messages.WebApi.Consumers;

public sealed class UserLoggedInConsumer(ILogger<UserLoggedInConsumer> logger, IMessageService messageService) : IConsumer<UserLoggedIn>
{
    public async Task Consume(ConsumeContext<UserLoggedIn> context)
    {
        var chatMessage = $"Привет, {context.Message.Email}!";

        await messageService.SendMessageAsync(Guid.NewGuid(), chatMessage);

        logger.LogInformation("Sent welcome message to chat for user {Email}", context.Message.Email);
    }
}
