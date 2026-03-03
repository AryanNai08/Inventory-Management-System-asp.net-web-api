using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(InventoryDbContext dbContext)
        {
            // Check if admin already exists
            bool adminExists = await dbContext.Users
                .AnyAsync(u => u.UserType == UserType.Admin && !u.IsDeleted);

            if (adminExists)
                return;

            // Hash the default password
            using var hmac = new HMACSHA512();
            byte[] passwordBytes = Encoding.UTF8.GetBytes("Admin@123");
            byte[] hashBytes = hmac.ComputeHash(passwordBytes);

            var admin = new User
            {
                Username = "admin",
                PasswordHash = Convert.ToBase64String(hashBytes),
                PasswordSalt = Convert.ToBase64String(hmac.Key),
                FullName = "System Administrator",
                Email = "admin@invms.com",
                UserType = UserType.Admin,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };

            await dbContext.Users.AddAsync(admin);
            await dbContext.SaveChangesAsync();
        }
    }
}
