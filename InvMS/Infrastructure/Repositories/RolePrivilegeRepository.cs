using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class RolePrivilegeRepository : IRolePrivilegeRepository
    {
        private readonly InventoryDbContext _dbContext;

        public RolePrivilegeRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Role> GetRoleWithPrivilegesAsync(int roleId)
        {
            return await _dbContext.Roles
                .Include(r => r.Privileges)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        public async Task<Privilege> GetPrivilegeByIdAsync(int privilegeId)
        {
            return await _dbContext.Privileges
                .FirstOrDefaultAsync(p => p.Id == privilegeId);
        }

        public async Task UpdateRoleAsync(Role role)
        {
            _dbContext.Roles.Update(role);
            await _dbContext.SaveChangesAsync();
        }
    }
}
