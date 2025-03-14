using Microsoft.EntityFrameworkCore;
using Tandia.Messages.Models;

namespace Tandia.Messages
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Message> Messages { get; set; }  
    }
}
