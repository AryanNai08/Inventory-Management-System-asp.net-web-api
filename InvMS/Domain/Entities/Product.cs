using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? PurchasePrice { get; set; }
    public decimal? SalePrice { get; set; }

    public int ReorderLevel { get; set; }
    public int CategoryId { get; set; }

    public int? SupplierId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    public virtual Supplier? Supplier { get; set; }
}
