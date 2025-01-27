using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { 
            this.Database.SetCommandTimeout(60);
        }

        public DbSet<User> Users { get; set; }
    }
}
