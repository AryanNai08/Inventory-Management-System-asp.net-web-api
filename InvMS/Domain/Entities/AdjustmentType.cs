using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AdjustmentType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();
}
