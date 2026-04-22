using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
using Domain.Enums;
using Domain.Models;

namespace Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly InventoryDbContext _dbContext;
        public ProductRepository(InventoryDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public async Task<PaginatedResult<ProductReadModel>> GetProjectedAllAsync(PaginationParams @params)
        {
            var baseQuery = _dbContext.Products
                .Where(p => !p.IsDeleted)
                .AsNoTracking();

            // 1. Calculate TotalCount
            var count = await baseQuery.CountAsync();

            // 2. Sorting
            if (!string.IsNullOrWhiteSpace(@params.SortColumn))
            {
                if (@params.SortColumn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    baseQuery = @params.SortOrder == "desc" ? baseQuery.OrderByDescending(p => p.Name) : baseQuery.OrderBy(p => p.Name);
                else if (@params.SortColumn.Equals("SalePrice", StringComparison.OrdinalIgnoreCase))
                    baseQuery = @params.SortOrder == "desc" ? baseQuery.OrderByDescending(p => p.SalePrice) : baseQuery.OrderBy(p => p.SalePrice);
                else
                    baseQuery = baseQuery.OrderBy(p => p.Id);
            }
            else
            {
                baseQuery = baseQuery.OrderBy(p => p.Id);
            }

            // 3. High-Performance Projection (to Domain Read Model)
            var items = await baseQuery
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .Select(p => new ProductReadModel {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    PurchasePrice = p.PurchasePrice,
                    SalePrice = p.SalePrice,
                    ReorderLevel = p.ReorderLevel,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier != null ? p.Supplier.Name : null,
                    TotalStock = p.ProductWarehouseStocks.Select(s => (int?)s.Quantity).Sum() ?? 0,
                    RowVersion = p.RowVersion
                })
                .ToListAsync();

            return new PaginatedResult<ProductReadModel>(items, count, @params.PageNumber, @params.PageSize);
        }

        public async Task<ProductReadModel?> GetProjectedByIdAsync(int id)
        {
            return await _dbContext.Products
                .Where(p => p.Id == id && !p.IsDeleted)
                .AsNoTracking()
                .Select(p => new ProductReadModel {
                    Id = p.Id,
                    Name = p.Name,
                    Sku = p.Sku,
                    Description = p.Description,
                    PurchasePrice = p.PurchasePrice,
                    SalePrice = p.SalePrice,
                    ReorderLevel = p.ReorderLevel,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier != null ? p.Supplier.Name : null,
                    TotalStock = p.ProductWarehouseStocks.Select(s => (int?)s.Quantity).Sum() ?? 0,
                    RowVersion = p.RowVersion
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
        }

        public async Task<PaginatedResult<Product>> GetAllAsync(PaginationParams @params)
        {
            var query = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => !p.IsDeleted)
                .AsQueryable();

            // Sorting
            if (!string.IsNullOrWhiteSpace(@params.SortColumn))
            {
                if (@params.SortColumn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                else if (@params.SortColumn.Equals("SalePrice", StringComparison.OrdinalIgnoreCase))
                    query = @params.SortOrder == "desc" ? query.OrderByDescending(p => p.SalePrice) : query.OrderBy(p => p.SalePrice);
                else
                    query = query.OrderBy(p => p.Id);
            }
            else
            {
                query = query.OrderBy(p => p.Id);
            }

            var count = await query.CountAsync();
            var items = await query
                .Skip((@params.PageNumber - 1) * @params.PageSize)
                .Take(@params.PageSize)
                .ToListAsync();

            return new PaginatedResult<Product>(items, count, @params.PageNumber, @params.PageSize);
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.Id == id && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

      
        public async Task<Product> GetBySkuAsync(string sku)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.Sku == sku && !p.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Product>> SearchAsync(string name, int? categoryId, int? supplierId)
        {
            var query = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
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
            // ModifiedDate and DeletedBy are now set automatically in DbContext.SaveChangesAsync
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
        }

        public async Task<List<Product>> GetLowStockAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.ProductWarehouseStocks)
                .Where(p => !p.IsDeleted && 
                           p.ProductWarehouseStocks.Sum(s => (int?)s.Quantity).GetValueOrDefault() <= p.ReorderLevel)
                .ToListAsync();
        }

        public async Task<List<Product>> GetOutOfStockAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.ProductWarehouseStocks)
                .Where(p => !p.IsDeleted && 
                           p.ProductWarehouseStocks.Sum(s => (int?)s.Quantity).GetValueOrDefault() == 0)
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
            return await _dbContext.ProductWarehouseStocks.AnyAsync(p => p.WarehouseId == warehouseId);
        }
    }
}
