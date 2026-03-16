using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PurchaseOrder
{
    public class CreatePurchaseOrderDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A valid Supplier is required")]
        public int SupplierId { get; set; }

        public DateTime? ExpectedDeliveryDate { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "A Purchase Order must have at least one Item")]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new List<CreatePurchaseOrderItemDto>();
    }
}
