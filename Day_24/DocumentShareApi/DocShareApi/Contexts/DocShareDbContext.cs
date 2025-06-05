using Microsoft.EntityFrameworkCore;
using DocShareApi.Models;

namespace DocShareApi.Data
{
    public class DocShareDbContext : DbContext
    {
        public DocShareDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Email as primary key
            modelBuilder.Entity<User>()
                .HasKey(u => u.Email);

            // One-to-many relationship: one user uploads many documents
            modelBuilder.Entity<Document>()
                .HasOne(d => d.Uploader)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK_User_Document")
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
