using Message.Db.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Message.Db.Services
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DatabaseContext _dbContext;

        public MessageRepository(DatabaseContext context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<Models.Message>> GetAllAsync()
        {
            return await _dbContext.Messages.ToListAsync();
        }

        public async Task AddAsync(Models.Message message)
        {
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            
        }
    }
}
