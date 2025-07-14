using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models;
using PropFinderApi.Contexts;
using PropFinderApi.Interfaces;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<PropFinderDbContext>();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminUserOptions>>();
            var hasher = scope.ServiceProvider.GetRequiredService<IEncryptionService>();
            var adminConfig = options.Value;

            var existingAdmin = await context.Users
                .FirstOrDefaultAsync(u => u.Email == adminConfig.Email && !u.IsDeleted);

            if (existingAdmin == null)
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    Email = adminConfig.Email,
                    Name = adminConfig.Name,
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = hasher.HashPassword(adminConfig.Password)
                };

                await context.Users.AddAsync(admin);
                await context.SaveChangesAsync();
            }

            if (!await context.FeatureMasters.AnyAsync())
            {
                var now = DateTime.UtcNow;

                var features = new List<FeatureMaster>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "BHK",
                        DataType = FeatureDataType.Dropdown,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "1 BHK" },
                            new() { Id = Guid.NewGuid(), Value = "2 BHK" },
                            new() { Id = Guid.NewGuid(), Value = "3 BHK" },
                            new() { Id = Guid.NewGuid(), Value = "4 BHK" },
                            new() { Id = Guid.NewGuid(), Value = "5 BHK" },
                            new() { Id = Guid.NewGuid(), Value = "6+ BHK" }
                        },
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Bathrooms",
                        DataType = FeatureDataType.Dropdown,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "1" },
                            new() { Id = Guid.NewGuid(), Value = "2" },
                            new() { Id = Guid.NewGuid(), Value = "3" },
                            new() { Id = Guid.NewGuid(), Value = "4" },
                            new() { Id = Guid.NewGuid(), Value = "5+" }
                        },
                        Applicability = ApplyToAll(
                            new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace },
                            new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Floor Number",
                        DataType = FeatureDataType.Dropdown,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "Ground" },
                            new() { Id = Guid.NewGuid(), Value = "1" },
                            new() { Id = Guid.NewGuid(), Value = "2" },
                            new() { Id = Guid.NewGuid(), Value = "3" },
                            new() { Id = Guid.NewGuid(), Value = "4" },
                            new() { Id = Guid.NewGuid(), Value = "5" },
                            new() { Id = Guid.NewGuid(), Value = "6+" }
                        },
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.CommercialSpace }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Facing",
                        DataType = FeatureDataType.Dropdown,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "North" },
                            new() { Id = Guid.NewGuid(), Value = "South" },
                            new() { Id = Guid.NewGuid(), Value = "East" },
                            new() { Id = Guid.NewGuid(), Value = "West" },
                            new() { Id = Guid.NewGuid(), Value = "North East" },
                            new() { Id = Guid.NewGuid(), Value = "North West" },
                            new() { Id = Guid.NewGuid(), Value = "South East" },
                            new() { Id = Guid.NewGuid(), Value = "South West" }
                        },
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace, PropertyType.Plot }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Furnishing",
                        DataType = FeatureDataType.Dropdown,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "Fully Furnished" },
                            new() { Id = Guid.NewGuid(), Value = "Semi Furnished" },
                            new() { Id = Guid.NewGuid(), Value = "Unfurnished" }
                        },
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Parking Availability",
                        DataType = FeatureDataType.MultiSelect,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        IsDeleted = false,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "2 Wheeler - Open" },
                            new() { Id = Guid.NewGuid(), Value = "2 Wheeler - Covered" },
                            new() { Id = Guid.NewGuid(), Value = "4 Wheeler - Open" },
                            new() { Id = Guid.NewGuid(), Value = "4 Wheeler - Covered" }
                        },
                        Applicability = ApplyToAll(
                            new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace },
                            new[] { ListingPurpose.Sale, ListingPurpose.Rent }
                        )
                    },


                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tenants Preferred",
                        DataType = FeatureDataType.MultiSelect,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "Bachelors" },
                            new() { Id = Guid.NewGuid(), Value = "Family" }
                        },
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House }, new[] { ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Amenities",
                        DataType = FeatureDataType.MultiSelect,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "Lift" },
                            new() { Id = Guid.NewGuid(), Value = "Gym" },
                            new() { Id = Guid.NewGuid(), Value = "Power Backup" },
                            new() { Id = Guid.NewGuid(), Value = "Swimming Pool" },
                            new() { Id = Guid.NewGuid(), Value = "Wi-Fi" },
                            new() { Id = Guid.NewGuid(), Value = "AC" },
                            new() { Id = Guid.NewGuid(), Value = "Piped Gas" },
                            new() { Id = Guid.NewGuid(), Value = "Vastu Compliance" },
                            new() { Id = Guid.NewGuid(), Value = "Security" }
                        },
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Is Negotiable",
                        DataType = FeatureDataType.Boolean,
                        FilterMode = FeatureFilterMode.Boolean,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace, PropertyType.Plot }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "EMI Available",
                        DataType = FeatureDataType.Boolean,
                        FilterMode = FeatureFilterMode.Boolean,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House }, new[] { ListingPurpose.Sale })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Deposit",
                        DataType = FeatureDataType.Number,
                        FilterMode = FeatureFilterMode.Range,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace, PropertyType.Plot }, new[] { ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Property Age (Years)",
                        DataType = FeatureDataType.Number,
                        FilterMode = FeatureFilterMode.Range,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House, PropertyType.CommercialSpace }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Gated Community",
                        DataType = FeatureDataType.Boolean,
                        FilterMode = FeatureFilterMode.Boolean,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House, PropertyType.Plot }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Road Facing",
                        DataType = FeatureDataType.Boolean,
                        FilterMode = FeatureFilterMode.Boolean,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = ApplyToAll(new[] { PropertyType.CommercialSpace }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Pet Friendly",
                        DataType = FeatureDataType.Boolean,
                        FilterMode = FeatureFilterMode.Boolean,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House }, new[] { ListingPurpose.Rent })
                    },

                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Water Supply Type",
                        DataType = FeatureDataType.MultiSelect,
                        FilterMode = FeatureFilterMode.Exact,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Options = new List<FeatureOption>
                        {
                            new() { Id = Guid.NewGuid(), Value = "Municipal" },
                            new() { Id = Guid.NewGuid(), Value = "Borewell" },
                            new() { Id = Guid.NewGuid(), Value = "Tank" }
                        },
                        Applicability = ApplyToAll(new[] { PropertyType.Apartment, PropertyType.House }, new[] { ListingPurpose.Sale, ListingPurpose.Rent })
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "DTCP/RERA Approved",
                        DataType = FeatureDataType.Boolean,
                        FilterMode = FeatureFilterMode.Boolean,
                        CreatedAt = now,
                        UpdatedAt = now,
                        Applicability = new List<FeatureApplicability>
                        {
                            new() { Id = Guid.NewGuid(), AppliesToType = PropertyType.Plot, AppliesToPurpose = ListingPurpose.Sale },
                            new() { Id = Guid.NewGuid(), AppliesToType = PropertyType.Plot, AppliesToPurpose = ListingPurpose.Rent }
                        }
                    },

                };

                await context.FeatureMasters.AddRangeAsync(features);
                await context.SaveChangesAsync();
            }
        }

        private static List<FeatureApplicability> ApplyToAll(PropertyType[] types, ListingPurpose[] purposes)
        {
            var list = new List<FeatureApplicability>();
            foreach (var type in types)
                foreach (var purpose in purposes)
                    list.Add(new FeatureApplicability
                    {
                        Id = Guid.NewGuid(),
                        AppliesToType = type,
                        AppliesToPurpose = purpose
                    });
            return list;
        }

    }
}
