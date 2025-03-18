namespace Tandia.Messages.Services;

using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Data;
using Tandia.Messages.Data.Entities;
using Tandia.Messages.Services.Interfaces;

public class MessageService : IMessageService
{
    private readonly DatabaseContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public MessageService(DatabaseContext context, TimeProvider timeProvider)
    {
        this._dbContext = context;
        this._timeProvider = timeProvider;
    }

    public async Task<IEnumerable<Message?>> GetAllAsync()
    {
        return await this._dbContext.Messages.ToListAsync();
    }

    public async Task<Message> SendMessageAsync(Guid id, Message message)
    {
        var _message = await this._dbContext.Messages.FirstOrDefaultAsync(x => x.Id == id);

        if (_message is null)
        {
            _message = new Message
            {
                Content = message.Content,
                Timestamp = _timeProvider.GetUtcNow(),
                LastModified = null,
            };

            await this._dbContext.Messages.AddAsync(_message);
            await this._dbContext.SaveChangesAsync();

            return _message;
        }

        _message.Content = message.Content;
        _message.LastModified = _timeProvider.GetUtcNow();

        this._dbContext.Messages.Update(_message);
        await this._dbContext.SaveChangesAsync();

        return _message;
    }

    public async Task UpdateMessageAsync(Message message)
    {
        this._dbContext.Messages.Update(message);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task<Message?> GetMessageByidAsync(Guid id)
    {
        return await this._dbContext.Messages.FirstOrDefaultAsync(x => x.Id == id);
    }
}
