using System;
using System.Collections.Generic;

namespace  Domain.Entities;


public partial class PurchaseOrderStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
