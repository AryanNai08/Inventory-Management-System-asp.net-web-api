using Application.DTOs.Dashboard;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
                
                // Low Stock: Products where current stock is at or below reorder level
                LowStockItemsCount = await _dbContext.Products
                    .CountAsync(p => !p.IsDeleted && p.CurrentStock <= p.ReorderLevel),

                // Revenue: Sum of all non-cancelled Sales Orders
                TotalSales = await _dbContext.SalesOrders
                    .Where(so => so.StatusId != 5) // 5 = Cancelled
                    .SumAsync(so => so.TotalAmount),

                // Investment: Sum of all Received Purchase Orders
                TotalPurchases = await _dbContext.PurchaseOrders
                    .Where(po => po.StatusId == 3) // 3 = Received
                    .SumAsync(po => po.TotalAmount)
            };

            // Get Top 5 selling products for the summary
            summary.TopSellingProducts = await GetTopSellingProductsAsync(5);

            return summary;
        }

        public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int count)
        {
            return await _dbContext.SalesOrderItems
                .Where(items => items.SalesOrder.StatusId != 5) // Exclude cancelled orders
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
    }
}
