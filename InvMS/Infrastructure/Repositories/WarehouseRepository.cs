using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly InventoryDbContext _dbContext;

        public WarehouseRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Warehouse warehouse)
        {
            await _dbContext.AddAsync(warehouse);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Warehouse>> GetAllAsync()
        {
            return await _dbContext.Warehouses.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<Warehouse> GetByIdAsync(int id)
        {
            return await _dbContext.Warehouses.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Warehouse> GetByNameAsync(string name)
        {
            return await _dbContext.Warehouses.Where(c => c.Name == name && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var warehouse = await _dbContext.Warehouses.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
            warehouse.IsDeleted = true;
            // ModifiedDate and DeletedBy are handled by DbContext.SaveChangesAsync automatically
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Warehouse warehouse)
        {
            _dbContext.Warehouses.Update(warehouse);
            await _dbContext.SaveChangesAsync();
        }
    }
}
