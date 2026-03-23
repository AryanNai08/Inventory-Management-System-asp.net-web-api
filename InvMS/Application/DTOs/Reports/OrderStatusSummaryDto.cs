namespace Application.DTOs.Reports
{
    public class OrderStatusSummaryDto
    {
        public string StatusName { get; set; } = null!;
        public int PurchaseOrderCount { get; set; }
        public int SalesOrderCount { get; set; }
    }
}
