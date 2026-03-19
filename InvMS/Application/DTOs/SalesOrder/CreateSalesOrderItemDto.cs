using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.SalesOrder
{
    public class CreateSalesOrderItemDto
    {
        //ProductId int[Required], [Range(1, int.MaxValue)]
        //Quantity int[Required], [Range(1, 100000)]
        //UnitPrice decimal[Required], [Range(0.01, 1000000)]

        [Required]
        [Range(1,int.MaxValue)]
        public int ProductId { get; set; }
        [Required]
        [Range (1,100000)]
        public int Quantity { get; set; }
        [Required]
        [Range(0.01,1000000)]
        public decimal UnitPrice { get; set; }
    }
}
