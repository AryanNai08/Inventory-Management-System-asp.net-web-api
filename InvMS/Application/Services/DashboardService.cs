using Application.DTOs.Dashboard;
using Application.DTOs.Reports;
using Domain.Interfaces;
using Domain.Exceptions;
using Domain.Common.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Interfaces;
using Application.Interfaces.Dashboard;

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
            var data = await _dashboardRepository.GetSummaryStatsAsync();

            return new DashboardSummaryDto
            {
                TotalSales = data.TotalSales,
                TotalPurchases = data.TotalPurchases,
                TotalProducts = data.TotalProducts,
                TotalSuppliers = data.TotalSuppliers,
                TotalCustomers = data.TotalCustomers,
                LowStockItemsCount = data.LowStockItemsCount,
                TopSellingProducts = data.TopSellingProducts.Select(p => new TopProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    TotalQuantitySold = p.TotalQuantitySold,
                    TotalRevenue = p.TotalRevenue
                }).ToList()
            };
        }

        public async Task<List<LowStockDto>> GetLowStockReportAsync()
        {
            var data = await _dashboardRepository.GetLowStockReportAsync();

            return data.Select(p => new LowStockDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CurrentStock = p.CurrentStock,
                ReorderLevel = p.ReorderLevel,
                CategoryName = p.CategoryName
            }).ToList();
        }

        public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int count)
        {
            var data = await _dashboardRepository.GetTopSellingProductsAsync(count);

            return data.Select(p => new TopProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                TotalQuantitySold = p.TotalQuantitySold,
                TotalRevenue = p.TotalRevenue
            }).ToList();
        }

        // ===================== REPORTS =====================

        public async Task<List<SalesByProductReportDto>> GetSalesByProductReportAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new BadRequestException("Start date cannot be after end date.");

            var data = await _dashboardRepository.GetSalesByProductAsync(startDate, endDate);

            return data.Select(x => new SalesByProductReportDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Sku = x.Sku,
                TotalQuantitySold = x.TotalQuantitySold,
                TotalRevenue = x.TotalRevenue
            }).ToList();
        }

        public async Task<List<PurchasesBySupplierReportDto>> GetPurchasesBySupplierReportAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new BadRequestException("Start date cannot be after end date.");

            var data = await _dashboardRepository.GetPurchasesBySupplierAsync(startDate, endDate);

            return data.Select(x => new PurchasesBySupplierReportDto
            {
                SupplierId = x.SupplierId,
                SupplierName = x.SupplierName,
                TotalOrders = x.TotalOrders,
                TotalInvestment = x.TotalInvestment
            }).ToList();
        }

        public async Task<List<StockMovementReportDto>> GetStockMovementReportAsync(int year)
        {
            if (year < 2000 || year > DateTime.UtcNow.Year + 1)
                throw new BadRequestException("Invalid year provided.");

            var data = await _dashboardRepository.GetStockMovementAsync(year);

            return data.Select(x => new StockMovementReportDto
            {
                Month = x.Month,
                MonthName = x.MonthName,
                StockIn = x.StockIn,
                StockOut = x.StockOut,
                NetMovement = x.NetMovement
            }).ToList();
        }

        public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
                throw new BadRequestException("Start date cannot be after end date.");

            var data = await _dashboardRepository.GetRevenueAsync(startDate, endDate);

            return new RevenueReportDto
            {
                TotalRevenue = data.TotalRevenue,
                TotalCost = data.TotalCost,
                GrossProfit = data.GrossProfit,
                TotalOrdersCompleted = data.TotalOrdersCompleted
            };
        }

        public async Task<List<OrderStatusSummaryDto>> GetOrderStatusSummaryAsync()
        {
            var data = await _dashboardRepository.GetOrderStatusSummaryAsync();

            return data.Select(x => new OrderStatusSummaryDto
            {
                StatusName = x.StatusName,
                PurchaseOrderCount = x.PurchaseOrderCount,
                SalesOrderCount = x.SalesOrderCount
            }).ToList();
        }
    }
}
