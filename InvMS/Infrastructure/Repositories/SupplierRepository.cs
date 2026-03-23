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
    public class SupplierRepository : ISupplierRepository
    {
        private readonly InventoryDbContext _dbContext;

        public SupplierRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _dbContext.AddAsync(supplier);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _dbContext.Suppliers.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<Supplier> GetByIdAsync(int id)
        {
            return await _dbContext.Suppliers.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<Supplier> GetByNameAsync(string name)
        {
            return await _dbContext.Suppliers.Where(c => c.Name == name && !c.IsDeleted).FirstOrDefaultAsync();
        }

        public async Task<List<Product>> GetProductsBySupplierIdAsync(int supplierId)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.SupplierId == supplierId && !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersBySupplierIdAsync(int supplierId)
        {
            return await _dbContext.PurchaseOrders.Include(po=>po.Status).Where(po=>po.SupplierId==supplierId).ToListAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var supplier = await _dbContext.Suppliers.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
            supplier.IsDeleted = true;
            supplier.ModifiedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _dbContext.Suppliers.Update(supplier);
            await _dbContext.SaveChangesAsync();
        }
    }
}
