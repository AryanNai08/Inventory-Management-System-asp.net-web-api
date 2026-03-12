using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int ReorderLevel { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }      // from navigation
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }      // from navigation
        public int? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }     // from navigation
        public byte[] RowVersion { get; set; }         // client needs this for updates
    }

}
