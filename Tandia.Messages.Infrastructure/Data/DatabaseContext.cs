using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Infrastructure.Data.Entities;

namespace Tandia.Messages.Infrastructure.Data;

public sealed class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<MessageEntity> Messages { get; set; }
}
