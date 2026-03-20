using System;

namespace Application.DTOs.StockAdjustment
{
    public class StockAdjustmentDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductSku { get; set; } = null!;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = null!;
        public int QuantityChange { get; set; }
        public int AdjustmentTypeId { get; set; }
        public string AdjustmentTypeName { get; set; } = null!;
        public string? Reason { get; set; }
        public int AdjustedBy { get; set; }
        public string AdjustedByUserName { get; set; } = null!;
        public DateTime AdjustmentDate { get; set; }
    }
}
