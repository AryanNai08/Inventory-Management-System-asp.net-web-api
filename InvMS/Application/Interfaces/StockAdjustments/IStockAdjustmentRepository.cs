using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common;

namespace Application.Interfaces
{
    public interface IStockAdjustmentRepository
    {
        Task<PaginatedResult<StockAdjustment>> GetAllAsync(PaginationParams @params);
        Task<StockAdjustment> GetByIdAsync(int id);
        Task<List<StockAdjustment>> GetByProductIdAsync(int productId);
        Task AddAsync(StockAdjustment adjustment);
        Task<List<AdjustmentType>> GetAdjustmentTypesAsync();
    }
}
