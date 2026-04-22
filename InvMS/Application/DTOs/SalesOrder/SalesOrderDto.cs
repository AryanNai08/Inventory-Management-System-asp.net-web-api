using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.SalesOrder
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public List<string> Warnings { get; set; } = new();

        public List<SalesOrderItemDto> Items {  get; set; }
    }
}
