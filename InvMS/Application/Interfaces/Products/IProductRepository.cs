using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
namespace Application.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();
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
