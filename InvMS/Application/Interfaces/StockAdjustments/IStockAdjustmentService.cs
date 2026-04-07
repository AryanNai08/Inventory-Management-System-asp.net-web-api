using Application.DTOs.StockAdjustment;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Common;

namespace Application.Interfaces.StockAdjustments
{
    public interface IStockAdjustmentService
    {
        Task<PaginatedResult<StockAdjustmentDto>> GetAllAsync(PaginationParams @params);
        Task<StockAdjustmentDto> GetByIdAsync(int id);
        Task<List<StockAdjustmentDto>> GetByProductIdAsync(int productId);
        Task<StockAdjustmentDto> CreateAsync(CreateStockAdjustmentDto dto, int userId);
        Task<List<AdjustmentTypeDto>> GetAdjustmentTypesAsync();
    }
}
