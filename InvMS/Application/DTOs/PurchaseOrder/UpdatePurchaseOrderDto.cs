using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PurchaseOrder
{
    public class UpdatePurchaseOrderDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A valid Supplier is required")]
        public int SupplierId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A valid Warehouse is required")]
        public int WarehouseId { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A Purchase Order must have at least one Item")]
        public List<UpdatePurchaseOrderItemDto> Items { get; set; } = new List<UpdatePurchaseOrderItemDto>();
    }

    public class UpdatePurchaseOrderItemDto
    {
        public int? Id { get; set; } // Null if it's a new item being added

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
