using Tandia.Messages.Application.Enums;
using Tandia.Messages.Application.Models;

namespace Tandia.Messages.Application.Services.Interfaces;

public interface IMessageService
{
    public Task<IReadOnlyCollection<Message?>> GetAllAsync();

    public Task<MessageStatus> SendMessageAsync(Guid id, string content);
}
