using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Application.Enums;
using Tandia.Messages.Application.Models;
using Tandia.Messages.Application.Services.Interfaces;
using Tandia.Messages.Infrastructure.Data;
using Tandia.Messages.Infrastructure.Data.Entities;

namespace Tandia.Messages.Application.Services;

public sealed class MessageService : IMessageService
{
    private readonly DatabaseContext dbContext;
    private readonly TimeProvider timeProvider;

    public MessageService(DatabaseContext context, TimeProvider provider)
    {
        dbContext = context;
        timeProvider = provider;
    }

    public async Task<IReadOnlyCollection<Message?>> GetAllAsync()
    {
        var messageEntities = await dbContext.Messages
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        var messages = messageEntities.ConvertAll(ConvertToMessage);

        return messages.AsReadOnly();
    }

    public async Task<MessageStatus> SendMessageAsync(Guid id, string content)
    {
        var message = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == id);

        if (message is null)
        {
            message = new MessageEntity(content)
            {
                Timestamp = timeProvider.GetUtcNow(),
                LastModified = null,
            };

            await dbContext.Messages.AddAsync(message);
            await dbContext.SaveChangesAsync();

            return MessageStatus.Created;
        }

        message.Content = content;
        message.LastModified = timeProvider.GetUtcNow();

        await dbContext.SaveChangesAsync();

        return MessageStatus.Updated;
    }

    private static Message ConvertToMessage(MessageEntity entity)
    {
        return new Message(entity.Id, entity.Content, entity.Timestamp, entity.LastModified);
    }
}
