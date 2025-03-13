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

        public async Task UpdateMessageAsync(Models.Message message)
        {
            _dbContext.Messages.Update(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Models.Message> GetMessageByid(Guid id)
        {
            return await _dbContext.Messages.FirstOrDefaultAsync(x  => x.Id == id);
        }
    }
}
