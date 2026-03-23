using Application.DTOs.Dashboard;
using Application.DTOs.Reports;
using Application.Interfaces;
using Domain.Exceptions;
using System;
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

        // ===================== REPORTS =====================

        public async Task<List<SalesByProductReportDto>> GetSalesByProductReportAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new BadRequestException("Start date cannot be after end date.");

            return await _dashboardRepository.GetSalesByProductAsync(startDate, endDate);
        }

        public async Task<List<PurchasesBySupplierReportDto>> GetPurchasesBySupplierReportAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new BadRequestException("Start date cannot be after end date.");

            return await _dashboardRepository.GetPurchasesBySupplierAsync(startDate, endDate);
        }

        public async Task<List<StockMovementReportDto>> GetStockMovementReportAsync(int year)
        {
            if (year < 2000 || year > DateTime.UtcNow.Year + 1)
                throw new BadRequestException("Invalid year provided.");

            return await _dashboardRepository.GetStockMovementAsync(year);
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new BadRequestException("Start date cannot be after end date.");

            return await _dashboardRepository.GetRevenueAsync(startDate, endDate);
        }

        public async Task<List<OrderStatusSummaryDto>> GetOrderStatusSummaryAsync()
        {
            return await _dashboardRepository.GetOrderStatusSummaryAsync();
        }
    }
}
