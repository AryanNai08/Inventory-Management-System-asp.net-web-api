using System;

namespace Application.DTOs.Warehouse
{
    public class WarehouseStockDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductSku { get; set; }
        public int Quantity { get; set; }
    }
}
