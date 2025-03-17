namespace Tandia.Messages.Services;

using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Data;
using Tandia.Messages.Data.Entities;
using Tandia.Messages.Services.Interfaces;

public class MessageRepository : IMessageRepository
{
    private readonly DatabaseContext _dbContext;

    public MessageRepository(DatabaseContext context)
    {
        this._dbContext = context;
    }

    public async Task<IEnumerable<Message?>> GetAllAsync()
    {
        return await this._dbContext.Messages.ToListAsync();
    }

    public async Task AddAsync(Message message)
    {
        await this._dbContext.Messages.AddAsync(message);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task UpdateMessageAsync(Message message)
    {
        this._dbContext.Messages.Update(message);
        await this._dbContext.SaveChangesAsync();
    }

    public async Task<Message?> GetMessageByid(Guid id)
    {
        return await this._dbContext.Messages.FirstOrDefaultAsync(x => x.Id == id);
    }
}
