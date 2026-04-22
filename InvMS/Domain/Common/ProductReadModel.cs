namespace Domain.Models
{
    public class ProductReadModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Sku { get; set; } = null!;
        public string? Description { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? SalePrice { get; set; }
        public int ReorderLevel { get; set; }
        public string CategoryName { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? SupplierName { get; set; }
        public int? SupplierId { get; set; }
        public int TotalStock { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public byte[] RowVersion { get; set; } = null!;
    }
}
