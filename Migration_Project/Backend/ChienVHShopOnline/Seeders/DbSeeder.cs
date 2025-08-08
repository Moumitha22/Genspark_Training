using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Seeders
{
    public static class DbSeeder
    {
        public static async Task SeedBasicEntitiesAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();


            // Seed Users
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new()
                    {
                        Username = "chienvh",
                        Email = "chienvh@gmail.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Chienvh@123"),
                        Role = UserRole.User
                    },
                    new()
                    {
                        Username = "admin",
                        Email = "admin@gmail.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        Role = UserRole.Admin
                    }
                };


                await context.Users.AddRangeAsync(users);
            }

            // Seed Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Iphone" },
                    new() { Name = "Ipad" },
                    new() { Name = "Macbook" }
                };

                await context.Categories.AddRangeAsync(categories);
            }

            // Seed Colors
            if (!await context.Colors.AnyAsync())
            {
                var colors = new List<Color>
                {
                    new() { Name = "Rose" },
                    new() { Name = "Gold" },
                    new() { Name = "White" },
                    new() { Name = "Black" },
                    new() { Name = "Grey" }
                };

                await context.Colors.AddRangeAsync(colors);
            }

            // Seed Models
            if (!await context.Models.AnyAsync())
            {
                var models = new List<Model>
                {
                    new() { Name = "6S Rose Gold" },
                    new() { Name = "6S Gold" },
                    new() { Name = "6 Gold" },
                    new() { Name = "6 Grey" },
                    new() { Name = "6S Rose Plus" },
                    new() { Name = "6S Gold Plus" },
                    new() { Name = "6 Gold Plus" },
                    new() { Name = "6 Grey Plus" },
                    new() { Name = "5S Gold" },
                    new() { Name = "5S Black" },
                    new() { Name = "5S White" },
                    new() { Name = "5 White" },
                    new() { Name = "5 Black" },
                    new() { Name = "Ipad Mini 1" },
                    new() { Name = "Ipad Mini 2" },
                    new() { Name = "Ipad Mini 3" },
                    new() { Name = "Ipad Mini 4" },
                    new() { Name = "Ipad 2" },
                    new() { Name = "Ipad 3" },
                    new() { Name = "Ipad 4" },
                    new() { Name = "Ipad Air" },
                    new() { Name = "Macbook Pro" },
                    new() { Name = "Macbook Pro Retina" }
                };


                await context.Models.AddRangeAsync(models);
            }

            // Seed Storages
            if (!await context.Storages.AnyAsync())
            {
                var storages = new List<Storage>
                {
                    new() { Name = "16 GB" },
                    new() { Name = "64 GB" },
                    new() { Name = "128 GB" },
                    new() { Name = "8 GB" },
                    new() { Name = "32 GB" }
                };

                await context.Storages.AddRangeAsync(storages);
            }

            if (!await context.Products.AnyAsync())
            {
                var user = await context.Users.FirstAsync(u => u.Username == "chienvh");

                var categoryMacbook = await context.Categories.FirstAsync(c => c.Name == "Macbook");
                var categoryIphone = await context.Categories.FirstAsync(c => c.Name == "Iphone");

                var modelRetina = await context.Models.FirstAsync(m => m.Name == "Macbook Pro Retina");
                var modelPro = await context.Models.FirstAsync(m => m.Name == "Macbook Pro");
                var model6SGold = await context.Models.FirstAsync(m => m.Name == "6S Gold");
                var model6Grey = await context.Models.FirstAsync(m => m.Name == "6 Grey");

                var colorGrey = await context.Colors.FirstAsync(c => c.Name == "Grey");
                var colorWhite = await context.Colors.FirstAsync(c => c.Name == "White");
                var colorBlack = await context.Colors.FirstAsync(c => c.Name == "Black");

                var storage16 = await context.Storages.FirstAsync(s => s.Name == "16 GB");
                var storage32 = await context.Storages.FirstAsync(s => s.Name == "32 GB");
                var storage128 = await context.Storages.FirstAsync(s => s.Name == "128 GB");

                var products = new List<Product>
                {
                    new()
                    {
                        ProductName = "Macbook Pro Retina 16GB Grey",
                        Price = 1899,
                        CategoryId = categoryMacbook.Id,
                        ModelId = modelRetina.Id,
                        ColorId = colorGrey.Id,
                        StorageId = storage16.Id,
                        UserId = user.Id,
                        SellStartDate = DateTime.UtcNow.AddDays(-5),
                        SellEndDate = DateTime.UtcNow.AddMonths(3),
                        IsNew = 1,
                        Image = "images/products/macbook1.jpeg"
                    },
                    new()
                    {
                        ProductName = "Macbook Pro 32GB White",
                        Price = 1999,
                        CategoryId = categoryMacbook.Id,
                        ModelId = modelPro.Id,
                        ColorId = colorWhite.Id,
                        StorageId = storage32.Id,
                        UserId = user.Id,
                        SellStartDate = DateTime.UtcNow.AddDays(-2),
                        SellEndDate = DateTime.UtcNow.AddMonths(3),
                        IsNew = 1,
                        Image = "images/products/macbook2.jpg"
                    },
                    new()
                    {
                        ProductName = "iPhone 6S Gold 128GB Black",
                        Price = 899,
                        CategoryId = categoryIphone.Id,
                        ModelId = model6SGold.Id,
                        ColorId = colorBlack.Id,
                        StorageId = storage128.Id,
                        UserId = user.Id,
                        SellStartDate = DateTime.UtcNow.AddDays(-3),
                        SellEndDate = DateTime.UtcNow.AddMonths(2),
                        IsNew = 1,
                        Image = "images/products/iphone1.jpeg"
                    },
                    new()
                    {
                        ProductName = "iPhone 6 Grey 32GB White",
                        Price = 749,
                        CategoryId = categoryIphone.Id,
                        ModelId = model6Grey.Id,
                        ColorId = colorWhite.Id,
                        StorageId = storage32.Id,
                        UserId = user.Id,
                        SellStartDate = DateTime.UtcNow,
                        SellEndDate = DateTime.UtcNow.AddMonths(1),
                        IsNew = 1,
                        Image = "images/products/iphone2.jpeg"
                    },
                    new()
                    {
                        ProductName = "Macbook Pro Retina 128GB Black",
                        Price = 2299,
                        CategoryId = categoryMacbook.Id,
                        ModelId = modelRetina.Id,
                        ColorId = colorBlack.Id,
                        StorageId = storage128.Id,
                        UserId = user.Id,
                        SellStartDate = DateTime.UtcNow.AddDays(-4),
                        SellEndDate = DateTime.UtcNow.AddMonths(3),
                        IsNew = 1,
                        Image = "images/products/ipad1.jpeg"
                    },
                    new()
                    {
                        ProductName = "iPhone 6S Gold 16GB Grey",
                        Price = 699,
                        CategoryId = categoryIphone.Id,
                        ModelId = model6SGold.Id,
                        ColorId = colorGrey.Id,
                        StorageId = storage16.Id,
                        UserId = user.Id,
                        SellStartDate = DateTime.UtcNow.AddDays(-1),
                        SellEndDate = DateTime.UtcNow.AddMonths(2),
                        IsNew = 1,
                        Image = "images/products/ipad2.jpeg"
                    }
                };

                await context.Products.AddRangeAsync(products);
            }
            
            await context.SaveChangesAsync();

        }
        
    }
}
