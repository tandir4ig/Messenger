using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Infrastructure.Data.Entities;
using Tandia.Messages.Infrastructure.Data.EntityConfigurations;

namespace Tandia.Messages.Infrastructure.Data;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<MessageEntity> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MessageEntityConfiguration());
    }
}
