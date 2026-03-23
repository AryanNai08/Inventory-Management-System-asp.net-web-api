using Application.DTOs.Dashboard;
using Application.DTOs.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardSummaryDto> GetSummaryStatsAsync();
        Task<List<TopProductDto>> GetTopSellingProductsAsync(int count);
        Task<List<LowStockDto>> GetLowStockReportAsync();

        // Reports
        Task<List<SalesByProductReportDto>> GetSalesByProductAsync(DateTime? startDate, DateTime? endDate);
        Task<List<PurchasesBySupplierReportDto>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate);
        Task<List<StockMovementReportDto>> GetStockMovementAsync(int year);
        Task<RevenueReportDto> GetRevenueAsync(DateTime? startDate, DateTime? endDate);
        Task<List<OrderStatusSummaryDto>> GetOrderStatusSummaryAsync();
    }
}
