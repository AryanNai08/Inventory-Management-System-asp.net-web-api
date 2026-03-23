using Application.DTOs.Dashboard;
using Application.DTOs.Reports;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly InventoryDbContext _dbContext;

        public DashboardRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardSummaryDto> GetSummaryStatsAsync()
        {
            var summary = new DashboardSummaryDto
            {
                TotalProducts = await _dbContext.Products.CountAsync(p => !p.IsDeleted),
                TotalCustomers = await _dbContext.Customers.CountAsync(c => !c.IsDeleted),
                TotalSuppliers = await _dbContext.Suppliers.CountAsync(s => !s.IsDeleted),
                
                LowStockItemsCount = await _dbContext.Products
                    .CountAsync(p => !p.IsDeleted && p.CurrentStock <= p.ReorderLevel),

                TotalSales = await _dbContext.SalesOrders
                    .Where(so => so.StatusId != 5)
                    .SumAsync(so => so.TotalAmount),

                TotalPurchases = await _dbContext.PurchaseOrders
                    .Where(po => po.StatusId == 3)
                    .SumAsync(po => po.TotalAmount)
            };

            summary.TopSellingProducts = await GetTopSellingProductsAsync(5);
            return summary;
        }

        public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int count)
        {
            return await _dbContext.SalesOrderItems
                .Where(items => items.SalesOrder.StatusId != 5)
                .GroupBy(items => new { items.ProductId, items.Product.Name })
                .Select(group => new TopProductDto
                {
                    ProductId = group.Key.ProductId,
                    ProductName = group.Key.Name,
                    TotalQuantitySold = group.Sum(x => x.Quantity),
                    TotalRevenue = group.Sum(x => x.LineTotal ?? 0)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<LowStockDto>> GetLowStockReportAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.CurrentStock <= p.ReorderLevel)
                .Select(p => new LowStockDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    CurrentStock = p.CurrentStock,
                    ReorderLevel = p.ReorderLevel,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }

        // ===================== REPORTS =====================

        public async Task<List<SalesByProductReportDto>> GetSalesByProductAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.SalesOrderItems
                .Where(soi => soi.SalesOrder.StatusId != 5) // Exclude cancelled
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(soi => soi.SalesOrder.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(soi => soi.SalesOrder.OrderDate <= endDate.Value);

            return await query
                .GroupBy(soi => new { soi.ProductId, soi.Product.Name, soi.Product.Sku })
                .Select(g => new SalesByProductReportDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    Sku = g.Key.Sku,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.LineTotal ?? 0)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
        }

        public async Task<List<PurchasesBySupplierReportDto>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.PurchaseOrders
                .Where(po => po.StatusId == 3) // Only Received
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(po => po.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(po => po.OrderDate <= endDate.Value);

            return await query
                .GroupBy(po => new { po.SupplierId, po.Supplier.Name })
                .Select(g => new PurchasesBySupplierReportDto
                {
                    SupplierId = g.Key.SupplierId,
                    SupplierName = g.Key.Name,
                    TotalOrders = g.Count(),
                    TotalInvestment = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.TotalInvestment)
                .ToListAsync();
        }

        public async Task<List<StockMovementReportDto>> GetStockMovementAsync(int year)
        {
            // Stock In: Items received via Purchase Orders (Status = Received)
            var stockIn = await _dbContext.PurchaseOrderItems
                .Where(poi => poi.PurchaseOrder.StatusId == 3 && poi.PurchaseOrder.OrderDate.Year == year)
                .GroupBy(poi => poi.PurchaseOrder.OrderDate.Month)
                .Select(g => new { Month = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToListAsync();

            // Stock Out: Items sold via Sales Orders (Status != Cancelled)
            var stockOut = await _dbContext.SalesOrderItems
                .Where(soi => soi.SalesOrder.StatusId != 5 && soi.SalesOrder.OrderDate.Year == year)
                .GroupBy(soi => soi.SalesOrder.OrderDate.Month)
                .Select(g => new { Month = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToListAsync();

            // Merge into 12 months
            var result = new List<StockMovementReportDto>();
            for (int m = 1; m <= 12; m++)
            {
                var inQty = stockIn.FirstOrDefault(x => x.Month == m)?.Quantity ?? 0;
                var outQty = stockOut.FirstOrDefault(x => x.Month == m)?.Quantity ?? 0;
                result.Add(new StockMovementReportDto
                {
                    Month = m,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    StockIn = inQty,
                    StockOut = outQty,
                    NetMovement = inQty - outQty
                });
            }
            return result;
        }

        public async Task<RevenueReportDto> GetRevenueAsync(DateTime? startDate, DateTime? endDate)
        {
            var salesQuery = _dbContext.SalesOrders
                .Where(so => so.StatusId != 5) // Exclude cancelled
                .AsQueryable();

            var purchaseQuery = _dbContext.PurchaseOrders
                .Where(po => po.StatusId == 3) // Only Received
                .AsQueryable();

            if (startDate.HasValue)
            {
                salesQuery = salesQuery.Where(so => so.OrderDate >= startDate.Value);
                purchaseQuery = purchaseQuery.Where(po => po.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                salesQuery = salesQuery.Where(so => so.OrderDate <= endDate.Value);
                purchaseQuery = purchaseQuery.Where(po => po.OrderDate <= endDate.Value);
            }

            var totalRevenue = await salesQuery.SumAsync(so => so.TotalAmount);
            var totalCost = await purchaseQuery.SumAsync(po => po.TotalAmount);
            var totalOrders = await salesQuery.CountAsync();

            return new RevenueReportDto
            {
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                GrossProfit = totalRevenue - totalCost,
                TotalOrdersCompleted = totalOrders
            };
        }

        public async Task<List<OrderStatusSummaryDto>> GetOrderStatusSummaryAsync()
        {
            // Get PO counts by status
            var poStatuses = await _dbContext.PurchaseOrders
                .GroupBy(po => po.Status.Name)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToListAsync();

            // Get SO counts by status
            var soStatuses = await _dbContext.SalesOrders
                .GroupBy(so => so.Status.Name)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToListAsync();

            // Merge all status names
            var allStatuses = poStatuses.Select(x => x.StatusName)
                .Union(soStatuses.Select(x => x.StatusName))
                .Distinct();

            return allStatuses.Select(status => new OrderStatusSummaryDto
            {
                StatusName = status,
                PurchaseOrderCount = poStatuses.FirstOrDefault(x => x.StatusName == status)?.Count ?? 0,
                SalesOrderCount = soStatuses.FirstOrDefault(x => x.StatusName == status)?.Count ?? 0
            }).ToList();
        }
    }
}
