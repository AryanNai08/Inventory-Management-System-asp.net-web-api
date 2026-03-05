using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
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

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _dbContext.Roles.FindAsync(id);
        }
    }
}
