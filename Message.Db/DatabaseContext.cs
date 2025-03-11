using Microsoft.EntityFrameworkCore;

namespace Message.Db
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Models.Message> Messages { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
