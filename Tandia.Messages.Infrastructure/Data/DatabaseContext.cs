namespace Tandia.Messages.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Infrastructure.Data.Entities;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public DbSet<MessageEntity> Messages { get; set; }
}
