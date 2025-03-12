namespace Message.Db.Services.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Models.Message>> GetAllAsync();
        Task AddAsync (Models.Message message);
    }
}
