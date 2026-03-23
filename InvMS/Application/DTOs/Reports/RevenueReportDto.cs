namespace Application.DTOs.Reports
{
    public class RevenueReportDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public int TotalOrdersCompleted { get; set; }
    }
}
