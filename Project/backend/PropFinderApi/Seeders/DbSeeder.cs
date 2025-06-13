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

            if (existingAdmin != null) return;

            var admin = new User
            {
                Id = Guid.NewGuid(),
                Email = adminConfig.Email,
                Name = adminConfig.Name,
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            admin.PasswordHash = hasher.HashPassword(adminConfig.Password);

            await context.Users.AddAsync(admin);
            await context.SaveChangesAsync();
        }
    }
}
