using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ProductWarehouseStock
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int Quantity { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
    public virtual Warehouse Warehouse { get; set; } = null!;
}
