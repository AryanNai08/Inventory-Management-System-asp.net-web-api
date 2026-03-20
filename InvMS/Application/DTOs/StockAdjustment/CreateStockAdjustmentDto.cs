using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.StockAdjustment
{
    public class CreateStockAdjustmentDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        [Range(-1000000, 1000000)]
        public int QuantityChange { get; set; }

        [Required]
        public int AdjustmentTypeId { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
