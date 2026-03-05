using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(InventoryDbContext dbContext)
        {
            bool adminExists = await dbContext.Users
                .AnyAsync(u => u.Username == "admin" && !u.IsDeleted);

            if (adminExists)
                return;

            var admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "System Administrator",
                Email = "admin@invms.com",
                IsDeleted = false,
                CreatedDate = DateTime.UtcNow
            };

            await dbContext.Users.AddAsync(admin);
            await dbContext.SaveChangesAsync();

            var adminRole = await dbContext.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin");

            if (adminRole != null)
            {
                admin.Roles.Add(adminRole);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}