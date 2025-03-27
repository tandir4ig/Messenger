namespace Tandia.Messages.Application.Models;

public sealed class Message(Guid id, string content, DateTimeOffset timestamp, DateTimeOffset? lastModified)
{
    public Guid Id { get; } = id;

    public string Content { get; } = content;

    public DateTimeOffset Timestamp { get; } = timestamp;

    public DateTimeOffset? LastModified { get; } = lastModified;
}
