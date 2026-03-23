using System.Collections.Generic;

namespace Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public int TotalProducts { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalCustomers { get; set; }
        public int LowStockItemsCount { get; set; }
        public List<TopProductDto> TopSellingProducts { get; set; } = new List<TopProductDto>();
    }

    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class LowStockDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int CurrentStock { get; set; }
        public int ReorderLevel { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
