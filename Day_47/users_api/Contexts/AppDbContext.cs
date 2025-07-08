using Microsoft.EntityFrameworkCore;
using users_api.Models;

namespace users_api.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; } = null!;
    }
}
