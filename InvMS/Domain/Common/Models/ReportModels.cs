using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common.Models
{
    public class SalesByProductReport
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class PurchasesBySupplierReport
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public int TotalOrders { get; set; }
        public decimal TotalInvestment { get; set; }
    }

    public class StockMovementReport
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = null!;
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int NetMovement { get; set; }
    }

    public class RevenueReport
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public int TotalOrdersCompleted { get; set; }
    }

    public class OrderStatusSummary
    {
        public string StatusName { get; set; } = null!;
        public int PurchaseOrderCount { get; set; }
        public int SalesOrderCount { get; set; }
    }
}
