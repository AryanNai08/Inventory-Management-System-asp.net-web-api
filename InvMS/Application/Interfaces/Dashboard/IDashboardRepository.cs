using Application.Common.Models;   // ✅ add this
using Application.DTOs.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDashboardRepository
    {
        // ✅ FIXED (use Models instead of DTOs)
        Task<DashboardSummary> GetSummaryStatsAsync();
        Task<List<TopProduct>> GetTopSellingProductsAsync(int count);
        Task<List<LowStock>> GetLowStockReportAsync();

        // 🔸 KEEP AS-IS FOR NOW (we’ll fix later)
        Task<List<SalesByProductReport>> GetSalesByProductAsync(DateTime? startDate, DateTime? endDate);
        Task<List<PurchasesBySupplierReport>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate);
        Task<List<StockMovementReport>> GetStockMovementAsync(int year);
        Task<RevenueReport> GetRevenueAsync(DateTime? startDate, DateTime? endDate);
        Task<List<OrderStatusSummary>> GetOrderStatusSummaryAsync();
    }
}