using Domain.Common;
using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        Task<PaginatedResult<Product>> GetAllAsync(PaginationParams @params);
        Task<PaginatedResult<ProductReadModel>> GetProjectedAllAsync(PaginationParams @params);
        Task<ProductReadModel?> GetProjectedByIdAsync(int id);
        Task<Product> GetByIdAsync(int id);
        Task<Product> GetBySkuAsync(string sku);
        Task<List<Product>> SearchAsync(string name, int? categoryId, int? supplierId);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task SoftDeleteAsync(int id);
        Task<bool> ExistsByCategoryIdAsync(int categoryId);
        Task<bool> ExistsBySupplierIdAsync(int supplierId);
        Task<bool> ExistsByWarehouseIdAsync(int warehouseId);
        Task<List<Product>> GetLowStockAsync();
        Task<List<Product>> GetOutOfStockAsync();
    }
}
