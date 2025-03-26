using Tandia.Messages.Application.Models;

namespace Tandia.Messages.Application.Services.Interfaces;

public interface IMessageService
{
    public Task<IReadOnlyCollection<Message?>> GetAllAsync();

    public Task<Message> SendMessageAsync(Guid id, string content);
}
