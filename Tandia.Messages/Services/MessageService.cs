using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Application.Models;
using Tandia.Messages.Application.Services.Interfaces;
using Tandia.Messages.Infrastructure.Data;
using Tandia.Messages.Infrastructure.Data.Entities;

namespace Tandia.Messages.Application.Services;

public class MessageService : IMessageService
{
    private readonly DatabaseContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public MessageService(DatabaseContext context, TimeProvider timeProvider)
    {
        this._dbContext = context;
        this._timeProvider = timeProvider;
    }

    public async Task<IReadOnlyCollection<Message?>> GetAllAsync()
    {
        var messageEntities = await this._dbContext.Messages.ToListAsync();

        var messages = new List<Message?>();

        foreach (var entity in messageEntities)
        {
            var message = new Message
            {
                Id = entity.Id,
                Content = entity.Content,
                LastModified = entity.LastModified,
                Timestamp = entity.Timestamp,
            };

            messages.Add(message);
        }

        return messages.AsEnumerable().Reverse().ToList();
    }

    public async Task<Message> SendMessageAsync(Guid id, string content)
    {
        var _message = await this._dbContext.Messages.FirstOrDefaultAsync(x => x.Id == id);

        if (_message is null)
        {
            _message = new MessageEntity
            {
                Content = content,
                Timestamp = _timeProvider.GetUtcNow(),
                LastModified = null,
            };

            await this._dbContext.Messages.AddAsync(_message);
            await this._dbContext.SaveChangesAsync();

            return new Message
            {
                Id = _message.Id,
                Content = _message.Content,
                Timestamp = _message.Timestamp,
                LastModified = _message.LastModified,
            };
        }

        _message.Content = content;
        _message.LastModified = _timeProvider.GetUtcNow();

        this._dbContext.Messages.Update(_message);
        await this._dbContext.SaveChangesAsync();

        return new Message
        {
            Id = _message.Id,
            Content = _message.Content,
            Timestamp = _message.Timestamp,
            LastModified = _message.LastModified,
        };
    }
}
