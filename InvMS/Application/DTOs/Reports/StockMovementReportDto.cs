namespace Application.DTOs.Reports
{
    public class StockMovementReportDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = null!;
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public int NetMovement { get; set; }
    }
}
