using Application.Common.Models;
using Application.DTOs.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDashboardRepository
    {
        
        Task<DashboardSummary> GetSummaryStatsAsync();
        Task<List<TopProduct>> GetTopSellingProductsAsync(int count);
        Task<List<LowStock>> GetLowStockReportAsync();


        Task<List<SalesByProductReport>> GetSalesByProductAsync(DateTime? startDate, DateTime? endDate);
        Task<List<PurchasesBySupplierReport>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate);
        Task<List<StockMovementReport>> GetStockMovementAsync(int year);
        Task<RevenueReport> GetRevenueAsync(DateTime? startDate, DateTime? endDate);
        Task<List<OrderStatusSummary>> GetOrderStatusSummaryAsync();
    }
}