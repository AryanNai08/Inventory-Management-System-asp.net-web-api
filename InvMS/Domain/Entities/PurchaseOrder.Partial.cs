using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PurchaseOrder
{
    public int WarehouseId { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
}
