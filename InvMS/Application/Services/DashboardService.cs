using Application.DTOs.Dashboard;
using Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            return await _dashboardRepository.GetSummaryStatsAsync();
        }

        public async Task<List<LowStockDto>> GetLowStockReportAsync()
        {
            return await _dashboardRepository.GetLowStockReportAsync();
        }

        public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int count)
        {
            return await _dashboardRepository.GetTopSellingProductsAsync(count);
        }
    }
}
