using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PurchaseOrderItem
{
    public int Id { get; set; }

    public int PurchaseOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? LineTotal { get; set; }

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
    
    public virtual Product Product { get; set; } = null!;
}
