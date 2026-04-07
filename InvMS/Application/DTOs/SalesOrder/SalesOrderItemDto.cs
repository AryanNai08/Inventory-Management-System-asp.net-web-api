using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.SalesOrder
{
    public class SalesOrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku {  get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
