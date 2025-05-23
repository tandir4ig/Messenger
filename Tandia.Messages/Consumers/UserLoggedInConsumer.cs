using MassTransit;
using Microsoft.Extensions.Logging;
using Tandia.Identity.Contracts.Events;
using Tandia.Messages.Application.Services.Interfaces;

namespace Tandia.Messages.Application.Consumers;

public sealed class UserLoggedInConsumer(ILogger<UserLoggedInConsumer> logger, IMessageService messageService) : IConsumer<UserLoggedIn>
{
    public async Task Consume(ConsumeContext<UserLoggedIn> context)
    {
        var chatMessage = $"Привет, {context.Message.Email}!";

        await messageService.SendMessageAsync(context.MessageId ?? Guid.NewGuid(), chatMessage);

        logger.LogInformation("Sent welcome message to chat for user {Email}", context.Message.Email);
    }
}
