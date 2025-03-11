namespace Message.Db.Services.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Models.Message>> GetAll();
        Task Add (Models.Message message);
    }
}
