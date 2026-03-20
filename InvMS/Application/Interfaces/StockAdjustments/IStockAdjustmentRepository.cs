using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IStockAdjustmentRepository
    {
        Task<List<StockAdjustment>> GetAllAsync();
        Task<StockAdjustment> GetByIdAsync(int id);
        Task<List<StockAdjustment>> GetByProductIdAsync(int productId);
        Task AddAsync(StockAdjustment adjustment);
        Task<List<AdjustmentType>> GetAdjustmentTypesAsync();
    }
}
