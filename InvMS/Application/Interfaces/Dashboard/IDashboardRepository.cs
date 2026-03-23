using Application.DTOs.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetSummaryStatsAsync();
        Task<List<TopProductDto>> GetTopSellingProductsAsync(int count);
        Task<List<LowStockDto>> GetLowStockReportAsync();
    }
}
