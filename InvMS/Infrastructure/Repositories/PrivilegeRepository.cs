using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class PrivilegeRepository : IPrivilegeRepository
    {
        private readonly InventoryDbContext _dbContext;

        public PrivilegeRepository(InventoryDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public async Task CreatePrivilegeAsync(Privilege privilege)
        {
            await _dbContext.Privileges.AddAsync(privilege);
        }

        public async Task<List<Privilege>> GetAllPrivilegesAsync()
        {
            return await _dbContext.Privileges.ToListAsync();
        }

        public async Task<Privilege> GetPrivilegeByIdAsync(int id)
        {
            return await _dbContext.Privileges.FindAsync(id);
        }

        public async Task<bool> PrivilegeExistsAsync(string name)
        {
            return await _dbContext.Privileges.AnyAsync(p => p.Name == name);
        }

        public async Task UpdatePrivilegeAsync(Privilege privilege)
        {
            _dbContext.Privileges.Update(privilege);
        }

        public async Task DeletePrivilegeAsync(Privilege privilege)
        {
            _dbContext.Privileges.Remove(privilege);
        }
    }
}
