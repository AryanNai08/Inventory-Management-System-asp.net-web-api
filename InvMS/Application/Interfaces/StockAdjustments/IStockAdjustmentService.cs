using Application.DTOs.StockAdjustment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IStockAdjustmentService
    {
        Task<List<StockAdjustmentDto>> GetAllAsync();
        Task<StockAdjustmentDto> GetByIdAsync(int id);
        Task<List<StockAdjustmentDto>> GetByProductIdAsync(int productId);
        Task<StockAdjustmentDto> CreateAsync(CreateStockAdjustmentDto dto, int userId);
        Task<List<AdjustmentTypeDto>> GetAdjustmentTypesAsync();
    }
}
