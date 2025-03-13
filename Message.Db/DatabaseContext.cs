using Microsoft.EntityFrameworkCore;

namespace Message.Db
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Models.Message> Messages { get; set; }  
    }
}
