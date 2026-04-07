using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class SalesOrderStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
