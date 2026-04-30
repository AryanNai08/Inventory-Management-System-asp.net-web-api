using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ProductWarehouseStockRepository : IProductWarehouseStockRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductWarehouseStockRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductWarehouseStock?> GetByProductAndWarehouseAsync(int productId, int warehouseId)
        {
            return await _dbContext.ProductWarehouseStocks
                .FirstOrDefaultAsync(s => s.ProductId == productId && s.WarehouseId == warehouseId);
        }

        public async Task AddAsync(ProductWarehouseStock stock)
        {
            await _dbContext.ProductWarehouseStocks.AddAsync(stock);
        }

        public async Task UpdateAsync(ProductWarehouseStock stock)
        {
            _dbContext.ProductWarehouseStocks.Update(stock);
        }

        public async Task<List<ProductWarehouseStock>> GetByWarehouseAsync(int warehouseId)
        {
            return await _dbContext.ProductWarehouseStocks
                .Include(s => s.Product)
                .Where(s => s.WarehouseId == warehouseId)
                .ToListAsync();
        }

        public async Task<List<ProductWarehouseStock>> GetByProductAsync(int productId)
        {
            return await _dbContext.ProductWarehouseStocks
                .Include(s => s.Warehouse)
                .Where(s => s.ProductId == productId)
                .ToListAsync();
        }
    }
}
