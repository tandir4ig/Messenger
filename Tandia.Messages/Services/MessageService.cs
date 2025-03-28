using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Application.Enums;
using Tandia.Messages.Application.Models;
using Tandia.Messages.Application.Services.Interfaces;
using Tandia.Messages.Infrastructure.Data;
using Tandia.Messages.Infrastructure.Data.Entities;

namespace Tandia.Messages.Application.Services;

internal sealed class MessageService(DatabaseContext dbContext, TimeProvider timeProvider) : IMessageService
{
    public async Task<IReadOnlyCollection<Message>> GetAllAsync()
    {
        var messageEntities = await dbContext.Messages
            .OrderByDescending(m => m.Timestamp)
            .ToListAsync();

        var messages = messageEntities.ConvertAll(ConvertToMessage);

        return messages.AsReadOnly();
    }

    public async Task<MessageStatus> SendMessageAsync(Guid id, string content)
    {
        var message = await dbContext.Messages.FirstOrDefaultAsync(x => x.Id == id);

        if (message is null)
        {
            var messageEntity = new MessageEntity(id, content, timeProvider.GetUtcNow(), lastModified: null);

            await dbContext.Messages.AddAsync(messageEntity);
            await dbContext.SaveChangesAsync();

            return MessageStatus.Created;
        }

        if (message.Content == content)
        {
            return MessageStatus.NotUpdated;
        }

        message.Content = content;
        message.LastModified = timeProvider.GetUtcNow();

        await dbContext.SaveChangesAsync();

        return MessageStatus.Updated;
    }

    private static Message ConvertToMessage(MessageEntity entity) => new(entity.Id, entity.Content, entity.Timestamp, entity.LastModified);
}
