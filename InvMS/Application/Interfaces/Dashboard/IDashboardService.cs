using Application.DTOs.Dashboard;
using Application.DTOs.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<List<LowStockDto>> GetLowStockReportAsync();
        Task<List<TopProductDto>> GetTopSellingProductsAsync(int count);

        // Reports
        Task<List<SalesByProductReportDto>> GetSalesByProductReportAsync(DateTime? startDate, DateTime? endDate);
        Task<List<PurchasesBySupplierReportDto>> GetPurchasesBySupplierReportAsync(DateTime? startDate, DateTime? endDate);
        Task<List<StockMovementReportDto>> GetStockMovementReportAsync(int year);
        Task<RevenueReportDto> GetRevenueReportAsync(DateTime? startDate, DateTime? endDate);
        Task<List<OrderStatusSummaryDto>> GetOrderStatusSummaryAsync();
    }
}
