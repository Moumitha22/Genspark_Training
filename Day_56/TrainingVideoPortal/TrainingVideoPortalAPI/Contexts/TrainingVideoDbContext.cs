using Microsoft.EntityFrameworkCore;
using TrainingVideoPortalAPI.Models;

namespace TrainingVideoPortalAPI.Contexts
{
    public class TrainingVideoDbContext : DbContext
    {
        public TrainingVideoDbContext(DbContextOptions<TrainingVideoDbContext> options)
            : base(options) { }

        public DbSet<TrainingVideo> TrainingVideos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TrainingVideo>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }

    }
}
