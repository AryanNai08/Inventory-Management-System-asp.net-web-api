using System.Collections.Generic;

namespace Application.DTOs.Product
{
    public class ProductStockBreakdownDto
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductStockBreakdownResponseDto
    {
        public int TotalStock { get; set; }
        public List<ProductStockBreakdownDto> Locations { get; set; }
    }
}
