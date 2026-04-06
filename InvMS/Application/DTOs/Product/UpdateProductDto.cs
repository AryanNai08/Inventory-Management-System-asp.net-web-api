using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.Product
{
    public class UpdateProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Sku { get; set; }
        public string? Description { get; set; }
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }
        [Required]
        public byte[] RowVersion { get; set; }  // ← client MUST send this back
    }

}
