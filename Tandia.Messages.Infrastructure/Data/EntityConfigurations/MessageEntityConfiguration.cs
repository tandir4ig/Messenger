using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tandia.Messages.Infrastructure.Data.Entities;

namespace Tandia.Messages.Infrastructure.Data.EntityConfigurations;

internal sealed class MessageEntityConfiguration : IEntityTypeConfiguration<MessageEntity>
{
    public void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedNever()
            .IsRequired();
    }
}
