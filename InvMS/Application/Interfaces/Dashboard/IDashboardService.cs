using Application.DTOs.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<List<LowStockDto>> GetLowStockReportAsync();
        Task<List<TopProductDto>> GetTopSellingProductsAsync(int count);
    }
}
