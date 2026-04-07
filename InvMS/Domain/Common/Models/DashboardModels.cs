using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Models
{
    
        public class DashboardSummary
        {
            public decimal TotalSales { get; set; }
            public decimal TotalPurchases { get; set; }
            public int TotalProducts { get; set; }
            public int TotalSuppliers { get; set; }
            public int TotalCustomers { get; set; }
            public int LowStockItemsCount { get; set; }
            public List<TopProduct> TopSellingProducts { get; set; } = new();
        }

        public class TopProduct
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = null!;
            public int TotalQuantitySold { get; set; }
            public decimal TotalRevenue { get; set; }
        }

        public class LowStock
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = null!;
            public int CurrentStock { get; set; }
            public int ReorderLevel { get; set; }
            public string CategoryName { get; set; } = null!;
        }
    
}
