using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class StockAdjustment
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int WarehouseId { get; set; }

    public int QuantityChange { get; set; }

    public int AdjustmentTypeId { get; set; }

    public string? Reason { get; set; }

    public int AdjustedBy { get; set; }

    public DateTime AdjustmentDate { get; set; }

    public virtual AdjustmentType AdjustmentType { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
