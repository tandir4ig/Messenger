namespace Tandia.Messages.Data;

using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Data.Entities;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<Message> Messages { get; set; }
}
