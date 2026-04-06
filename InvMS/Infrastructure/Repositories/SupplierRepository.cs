using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Common;

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
        }

        public async Task<PaginatedResult<Supplier>> GetAllAsync(PaginationParams @params)
        {
            var query = _dbContext.Suppliers
                .Where(s => !s.IsDeleted)
                .AsQueryable();

            // Sorting
            if (!string.IsNullOrWhiteSpace(@params.SortColumn))
            {
                if (@params.SortColumn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
                else if (@params.SortColumn.Equals("City", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(s => s.City) : query.OrderBy(s => s.City);
                else
                    query = query.OrderBy(s => s.Id);
            }
            else
            {
                query = query.OrderBy(s => s.Id);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .ToListAsync();

            return new PaginatedResult<Supplier>(items, count, @params.PageNumber, @params.PageSize);
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
            // ModifiedDate and DeletedBy are now set automatically in DbContext.SaveChangesAsync
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _dbContext.Suppliers.Update(supplier);
        }
    }
}
