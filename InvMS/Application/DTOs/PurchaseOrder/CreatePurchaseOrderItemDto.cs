using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PurchaseOrder
{
    public class CreatePurchaseOrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Quantity must be between 1 and 100,000")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Unit cost must be greater than 0")]
        public decimal UnitCost { get; set; }
    }
}
