using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProductWarehouseStockRepository
    {
        Task<ProductWarehouseStock?> GetByProductAndWarehouseAsync(int productId, int warehouseId);
        Task AddAsync(ProductWarehouseStock stock);
        Task UpdateAsync(ProductWarehouseStock stock);
        Task<List<ProductWarehouseStock>> GetByWarehouseAsync(int warehouseId);
        Task<List<ProductWarehouseStock>> GetByProductAsync(int productId);
    }
}
