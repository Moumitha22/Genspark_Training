using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models;

namespace PropFinderApi.Contexts
{
    public class PropFinderDbContext : DbContext
    {
        public PropFinderDbContext(DbContextOptions<PropFinderDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }  
        public DbSet<AgentProfile> AgentProfiles { get; set; }  
        public DbSet<Property> Properties { get; set; }  
        public DbSet<PropertyImage> PropertyImages { get; set; }  
        public DbSet<ContactLog> ContactLogs { get; set; }   
        public DbSet<RefreshToken> RefreshTokens { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(user =>
            {
                user.HasKey(u => u.Id)
                    .HasName("PK_User");

                user.Property(u => u.Role)
                    .HasConversion<string>();

                user.HasOne(u => u.AgentProfile)
                    .WithOne(ap => ap.User)
                    .HasForeignKey<AgentProfile>(ap => ap.UserId)
                    .HasConstraintName("FK_AgentProfile_User");

                user.HasMany(u => u.Properties)
                    .WithOne(p => p.Agent)
                    .HasForeignKey(p => p.AgentId)
                    .HasConstraintName("FK_Property_Agent");

                user.HasMany(u => u.ContactRequestsMade)
                    .WithOne(cl => cl.Buyer)
                    .HasForeignKey(cl => cl.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ContactLog_Buyer");

                user.HasMany(u => u.ContactRequestsReceived)
                    .WithOne(cl => cl.Agent)
                    .HasForeignKey(cl => cl.AgentId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ContactLog_Agent");
            });

            // AgentProfile
            modelBuilder.Entity<AgentProfile>(agentProfile =>
            {
                agentProfile.HasKey(ap => ap.Id)
                            .HasName("PK_AgentProfile");
            });

            // Property
            modelBuilder.Entity<Property>(property =>
            {
                property.HasKey(p => p.Id)
                        .HasName("PK_Property");
                
                property.Property(p => p.PropertyType)
                        .HasConversion<string>();

                property.Property(p => p.ListingType)
                        .HasConversion<string>();

                property.HasMany(p => p.PropertyImages)
                        .WithOne(pi => pi.Property)
                        .HasForeignKey(pi => pi.PropertyId)
                        .HasConstraintName("FK_PropertyImage_Property");

                property.HasMany(p => p.ContactRequests)
                        .WithOne(cl => cl.Property)
                        .HasForeignKey(cl => cl.PropertyId)
                        .HasConstraintName("FK_ContactLog_Property");
            });

            // PropertyImage
            modelBuilder.Entity<PropertyImage>(propertyImage =>
            {
                propertyImage.HasKey(pi => pi.Id)
                            .HasName("PK_PropertyImage");
            });

            // ContactLog
            modelBuilder.Entity<ContactLog>(contactLog =>
            {
                contactLog.HasKey(cl => cl.Id)
                        .HasName("PK_ContactLog");
            });

            // RefreshToken
            modelBuilder.Entity<RefreshToken>(refreshToken =>
            {
                refreshToken.HasKey(rt => rt.Id)
                            .HasName("PK_RefreshToken");

                refreshToken.HasOne(rt => rt.User)
                            .WithMany()
                            .HasForeignKey(rt => rt.UserId)
                            .HasConstraintName("FK_RefreshToken_User");
            });
        }

    }
}
