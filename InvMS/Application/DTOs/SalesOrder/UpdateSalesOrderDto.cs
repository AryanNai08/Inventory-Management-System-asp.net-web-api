using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.SalesOrder
{
    public class UpdateSalesOrderDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        [MinLength(1)]
        public List<CreateSalesOrderItemDto> Items { get; set; }
    }
}
