namespace Application.DTOs.Reports
{
    public class PurchasesBySupplierReportDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public int TotalOrders { get; set; }
        public decimal TotalInvestment { get; set; }
    }
}
