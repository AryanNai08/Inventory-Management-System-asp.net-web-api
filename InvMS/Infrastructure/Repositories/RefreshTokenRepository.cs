using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly InventoryDbContext _dbContext;

        public RefreshTokenRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _dbContext.RefreshTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> TokenIdExist(int id)
        {
            return await _dbContext.RefreshTokens.AnyAsync(s => s.Id == id);
        }

        public async Task<RefreshToken?> GetByIdAsync(int id)
        {
            return await _dbContext.RefreshTokens.FindAsync(id);
        }

        public async Task<RefreshToken?> GetByUserIdAsync(int userId)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.UserId == userId);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token && (t.IsRevoked ?? false) == false);
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _dbContext.RefreshTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
