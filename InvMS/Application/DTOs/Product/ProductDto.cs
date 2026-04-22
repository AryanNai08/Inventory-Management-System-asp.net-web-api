using System;
using Domain.Enums;

namespace Application.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string? Description { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int ReorderLevel { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int TotalStock { get; set; }
        public StockStatus StockStatus { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string RowVersion { get; set; }
    }
}
