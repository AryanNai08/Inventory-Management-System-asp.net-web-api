using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly InventoryDbContext _dbContext;

        public RoleRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await _dbContext.Roles.AnyAsync(r => r.Name == roleName);
        }

        public async Task CreateRoleAsync(Role role)
        {
            await _dbContext.Roles.AddAsync(role);
        }

       
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _dbContext.Roles.ToListAsync();

        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _dbContext.Roles.FindAsync(id);
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        

        public async Task UpdateRoleAsync(Role role)
        {
            _dbContext.Roles.Update(role);
        }

        public async Task DeleteRoleAsync(Role role)
        {
            _dbContext.Roles.Remove(role);
        }
    }
}
