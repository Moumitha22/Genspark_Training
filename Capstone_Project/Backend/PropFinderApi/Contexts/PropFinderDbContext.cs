using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models;

namespace PropFinderApi.Contexts
{
    public class PropFinderDbContext : DbContext
    {
        public PropFinderDbContext(DbContextOptions<PropFinderDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ListerProfile> ListerProfiles { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyLocation> PropertyLocations { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<ContactLog> ContactLogs { get; set; }
        public DbSet<FeatureMaster> FeatureMasters { get; set; }
        public DbSet<FeatureOption> FeatureOptions { get; set; }
        public DbSet<FeatureApplicability> FeatureApplicabilities { get; set; }
        public DbSet<PropertyFeature> PropertyFeatures { get; set; }
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

                user.HasOne(u => u.ListerProfile)
                    .WithOne(lp => lp.User)
                    .HasForeignKey<ListerProfile>(lp => lp.UserId)
                    .HasConstraintName("FK_ListerProfile_User");

                user.HasMany(u => u.Properties)
                    .WithOne(p => p.Lister)
                    .HasForeignKey(p => p.ListerId)
                    .HasConstraintName("FK_Property_Lister");

                user.HasMany(u => u.ContactRequestsMade)
                    .WithOne(cl => cl.Buyer)
                    .HasForeignKey(cl => cl.BuyerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ContactLog_Buyer");

                user.HasMany(u => u.ContactRequestsReceived)
                    .WithOne(cl => cl.Lister)
                    .HasForeignKey(cl => cl.ListerId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ContactLog_Lister");
            });

            // ListerProfile
            modelBuilder.Entity<ListerProfile>(ListerProfile =>
            {
                ListerProfile.HasKey(lp => lp.Id)
                            .HasName("PK_ListerProfile");
            });

            // Property
            modelBuilder.Entity<Property>(property =>
            {
                property.HasKey(p => p.Id)
                        .HasName("PK_Property");

                property.Property(p => p.PropertyType)
                        .HasConversion<string>();

                property.Property(p => p.ListingPurpose)
                        .HasConversion<string>();

                property.Property(p => p.ListerType)
                    .HasConversion<string>();

                property.Property(p => p.Status)
                    .HasConversion<string>();

                property.HasOne(p => p.Location)
                        .WithOne(pl => pl.Property)
                        .HasForeignKey<PropertyLocation>(pl => pl.PropertyId)
                        .HasConstraintName("FK_PropertyLocation_Property");

                property.HasMany(p => p.PropertyImages)
                        .WithOne(pi => pi.Property)
                        .HasForeignKey(pi => pi.PropertyId)
                        .HasConstraintName("FK_PropertyImage_Property");

                property.HasMany(p => p.ContactRequests)
                        .WithOne(cl => cl.Property)
                        .HasForeignKey(cl => cl.PropertyId)
                        .HasConstraintName("FK_ContactLog_Property");

                property.HasMany(p => p.Features)
                        .WithOne(pf => pf.Property)
                        .HasForeignKey(pf => pf.PropertyId)
                        .HasConstraintName("FK_PropertyFeature_Property");
            });

            // PropertyLocation
            modelBuilder.Entity<PropertyLocation>(PropertyLocation =>
            {
                PropertyLocation.HasKey(pl => pl.Id)
                            .HasName("PK_PropertyLocation");
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

            // FEATURE MASTER
            modelBuilder.Entity<FeatureMaster>(featureMaster =>
            {
                featureMaster.HasKey(fm => fm.Id)
                    .HasName("PK_FeatureMaster");

                featureMaster.Property(fm => fm.DataType)
                    .HasConversion<string>();
            });

            // FEATURE OPTION
            modelBuilder.Entity<FeatureOption>(featureOption =>
            {
                featureOption.HasKey(fo => fo.Id)
                    .HasName("PK_FeatureOption");

                featureOption.HasOne(fo => fo.Feature)
                            .WithMany(fm => fm.Options)
                            .HasForeignKey(fo => fo.FeatureId)
                            .HasConstraintName("FK_FeatureOption_Feature");

            });

            // FEATURE APPLICABILITY
            modelBuilder.Entity<FeatureApplicability>(featureApplicability =>
            {
                featureApplicability.HasKey(fa => fa.Id).HasName("PK_FeatureApplicability");

                featureApplicability.Property(fa => fa.AppliesToType)
                    .HasConversion<string>();

                featureApplicability.Property(fa => fa.AppliesToPurpose)
                    .HasConversion<string>();

                featureApplicability.HasOne(fa => fa.Feature)
                                    .WithMany(fm => fm.Applicability)
                                    .HasForeignKey(fa => fa.FeatureId)
                                    .HasConstraintName("FK_FeatureAvailability_Feature");
            });

            // PROPERTY FEATURE
            modelBuilder.Entity<PropertyFeature>(propertyFeature =>
            {
                propertyFeature.HasKey(pf => pf.Id)
                    .HasName("PK_PropertyFeature");

                propertyFeature.HasOne(pf => pf.Feature)
                                .WithMany()
                                .HasForeignKey(pf => pf.FeatureId)
                                .HasConstraintName("FK_PropertyFeature_Feature");

                propertyFeature.HasOne(pf => pf.Option)
                                .WithMany()
                                .HasForeignKey(pf => pf.OptionId)
                                .IsRequired(false)
                                .HasConstraintName("FK_PropertyFeature_FeatureOption");
            });
        }

    }
}
