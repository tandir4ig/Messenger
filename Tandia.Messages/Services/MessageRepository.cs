using Tandia.Messages.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Models;

namespace Tandia.Messages.Services;

public class MessageRepository : IMessageRepository
{
    private readonly DatabaseContext _dbContext;

    public MessageRepository(DatabaseContext context)
    {
        _dbContext = context;
    }

    public async Task<IEnumerable<Message>> GetAllAsync()
    {
        return await _dbContext.Messages.ToListAsync();
    }

    public async Task AddAsync(Message message)
    {
        await _dbContext.Messages.AddAsync(message);
        await _dbContext.SaveChangesAsync();
        
    }

    public async Task UpdateMessageAsync(Message message)
    {
        _dbContext.Messages.Update(message);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Message> GetMessageByid(Guid id)
    {
        return await _dbContext.Messages.FirstOrDefaultAsync(x  => x.Id == id);
    }
}
