using System.ComponentModel.DataAnnotations;

namespace Tandia.Messages.Infrastructure.Data.Entities;

public sealed class MessageEntity
{
    public MessageEntity(Guid id, string content, DateTimeOffset timestamp, DateTimeOffset? lastModified)
    {
        Id = id;
        Content = content;
        Timestamp = timestamp;
        LastModified = lastModified;
    }

    public Guid Id { get; }

    [Required]
    public string Content { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
