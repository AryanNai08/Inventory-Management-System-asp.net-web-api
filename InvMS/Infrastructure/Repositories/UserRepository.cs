using Domain.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.Auth;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly InventoryDbContext _dbContext;

        public UserRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(User user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .ThenInclude(u=>u.Privileges)
                .Where(u => !u.IsDeleted)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .ThenInclude(u => u.Privileges)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User> GetByUseremailAsync(string email)
        {
            return await _dbContext.Users
                .Include(u => u.Roles)
                .ThenInclude(u => u.Privileges)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .Include(u => u.Roles).ThenInclude(u=>u.Privileges)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var user = await _dbContext.Users.Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            user.IsDeleted = true;
            // ModifiedDate and DeletedBy are handled by DbContext.SaveChangesAsync automatically

            _dbContext.Users.Update(user);
        }

        public async Task UpdateAsync(User user)
        {
            _dbContext.Users.Update(user);
        }
    }
}
