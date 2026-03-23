using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly InventoryDbContext _dbContext;
        public ProductRepository(InventoryDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

      
        public async Task<Product> GetBySkuAsync(string sku)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => p.Sku == sku && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Product>> SearchAsync(string name, int? categoryId, int? supplierId)
        {
            var query = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            
            var product = await _dbContext.Products.Where(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
            product.IsDeleted = true;
            product.ModifiedDate = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Product>> GetLowStockAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => !p.IsDeleted && p.CurrentStock <= p.ReorderLevel)
                .ToListAsync();
        }

        public async Task<List<Product>> GetOutOfStockAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => !p.IsDeleted && p.CurrentStock == 0)
                .ToListAsync();
        }

        public async Task<bool> ExistsByCategoryIdAsync(int categoryId)
        {
            return await _dbContext.Products.AnyAsync(p => p.CategoryId == categoryId && !p.IsDeleted);
        }

        public async Task<bool> ExistsBySupplierIdAsync(int supplierId)
        {
            return await _dbContext.Products.AnyAsync(p => p.SupplierId == supplierId && !p.IsDeleted);
        }

        public async Task<bool> ExistsByWarehouseIdAsync(int warehouseId)
        {
            return await _dbContext.Products.AnyAsync(p => p.WarehouseId == warehouseId && !p.IsDeleted);
        }
    }
}
