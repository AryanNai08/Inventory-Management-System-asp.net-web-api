using Domain.Common.Models;
using Domain.Interfaces;
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

        public async Task<DashboardSummary> GetSummaryStatsAsync()
        {
            // Combine all numerical stats into one single database round-trip
            // Using a dummy projection to execute multiple aggregations in one SQL query
            var stats = await _dbContext.Products
                .AsNoTracking()
                .Select(_ => new
                {
                    TotalProducts = _dbContext.Products.Count(p => !p.IsDeleted),
                    TotalCustomers = _dbContext.Customers.Count(c => !c.IsDeleted),
                    TotalSuppliers = _dbContext.Suppliers.Count(s => !s.IsDeleted),
                    
                    LowStockCount = _dbContext.Products.Count(p => !p.IsDeleted && 
                        p.ProductWarehouseStocks.Sum(s => (int?)s.Quantity) <= p.ReorderLevel),

                    TotalSales = _dbContext.SalesOrders
                        .Where(so => so.StatusId != 5)
                        .Sum(so => (decimal?)so.TotalAmount) ?? 0,

                    TotalPurchases = _dbContext.PurchaseOrders
                        .Where(po => po.StatusId == 3)
                        .Sum(po => (decimal?)po.TotalAmount) ?? 0
                })
                .FirstOrDefaultAsync() ?? new { 
                    TotalProducts = 0, TotalCustomers = 0, TotalSuppliers = 0, 
                    LowStockCount = 0, TotalSales = 0m, TotalPurchases = 0m 
                };

            // Top selling products requires a GroupBy which is better handled separately
            var topSellingProducts = await GetTopSellingProductsAsync(5);

            return new DashboardSummary
            {
                TotalProducts = stats.TotalProducts,
                TotalCustomers = stats.TotalCustomers,
                TotalSuppliers = stats.TotalSuppliers,
                LowStockItemsCount = stats.LowStockCount,
                TotalSales = stats.TotalSales,
                TotalPurchases = stats.TotalPurchases,
                TopSellingProducts = topSellingProducts
            };
        }

        public async Task<List<TopProduct>> GetTopSellingProductsAsync(int count)
        {
            return await _dbContext.SalesOrderItems
                .AsNoTracking()
                .Where(items => items.SalesOrder.StatusId != 5)
                .GroupBy(items => new { items.ProductId, items.Product.Name })
                .Select(group => new TopProduct
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

        public async Task<List<LowStock>> GetLowStockReportAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.ProductWarehouseStocks)
                .Where(p => !p.IsDeleted && 
                           p.ProductWarehouseStocks.Sum(s => (int?)s.Quantity).GetValueOrDefault() <= p.ReorderLevel)
                .Select(p => new LowStock
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    CurrentStock = p.ProductWarehouseStocks.Sum(s => (int?)s.Quantity).GetValueOrDefault(),
                    ReorderLevel = p.ReorderLevel,
                    CategoryName = p.Category.Name
                })
                .ToListAsync();
        }

        // ===================== REPORTS =====================

        public async Task<List<SalesByProductReport>> GetSalesByProductAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.SalesOrderItems
                .Where(soi => soi.SalesOrder.StatusId != 5)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(soi => soi.SalesOrder.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(soi => soi.SalesOrder.OrderDate <= endDate.Value);

            return await query
                .GroupBy(soi => new { soi.ProductId, soi.Product.Name, soi.Product.Sku })
                .Select(g => new SalesByProductReport
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

        public async Task<List<PurchasesBySupplierReport>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _dbContext.PurchaseOrders
                .Where(po => po.StatusId == 3)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(po => po.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(po => po.OrderDate <= endDate.Value);

            return await query
                .GroupBy(po => new { po.SupplierId, po.Supplier.Name })
                .Select(g => new PurchasesBySupplierReport
                {
                    SupplierId = g.Key.SupplierId,
                    SupplierName = g.Key.Name,
                    TotalOrders = g.Count(),
                    TotalInvestment = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.TotalInvestment)
                .ToListAsync();
        }

        public async Task<List<StockMovementReport>> GetStockMovementAsync(int year)
        {
            var stockIn = await _dbContext.PurchaseOrderItems
                .Where(poi => poi.PurchaseOrder.StatusId == 3 && poi.PurchaseOrder.OrderDate.Year == year)
                .GroupBy(poi => poi.PurchaseOrder.OrderDate.Month)
                .Select(g => new { Month = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToListAsync();

            var stockOut = await _dbContext.SalesOrderItems
                .Where(soi => soi.SalesOrder.StatusId != 5 && soi.SalesOrder.OrderDate.Year == year)
                .GroupBy(soi => soi.SalesOrder.OrderDate.Month)
                .Select(g => new { Month = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToListAsync();

            var result = new List<StockMovementReport>();

            for (int m = 1; m <= 12; m++)
            {
                var inQty = stockIn.FirstOrDefault(x => x.Month == m)?.Quantity ?? 0;
                var outQty = stockOut.FirstOrDefault(x => x.Month == m)?.Quantity ?? 0;

                result.Add(new StockMovementReport
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

        public async Task<RevenueReport> GetRevenueAsync(DateTime? startDate, DateTime? endDate)
        {
            var salesQuery = _dbContext.SalesOrders
                .Where(so => so.StatusId != 5)
                .AsQueryable();

            var purchaseQuery = _dbContext.PurchaseOrders
                .Where(po => po.StatusId == 3)
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

            return new RevenueReport
            {
                TotalRevenue = totalRevenue,
                TotalCost = totalCost,
                GrossProfit = totalRevenue - totalCost,
                TotalOrdersCompleted = totalOrders
            };
        }

        public async Task<List<OrderStatusSummary>> GetOrderStatusSummaryAsync()
        {
            var poStatuses = await _dbContext.PurchaseOrders
                .GroupBy(po => po.Status.Name)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToListAsync();

            var soStatuses = await _dbContext.SalesOrders
                .GroupBy(so => so.Status.Name)
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToListAsync();

            var allStatuses = poStatuses.Select(x => x.StatusName)
                .Union(soStatuses.Select(x => x.StatusName))
                .Distinct();

            return allStatuses.Select(status => new OrderStatusSummary
            {
                StatusName = status,
                PurchaseOrderCount = poStatuses.FirstOrDefault(x => x.StatusName == status)?.Count ?? 0,
                SalesOrderCount = soStatuses.FirstOrDefault(x => x.StatusName == status)?.Count ?? 0
            }).ToList();
        }
    }
}
