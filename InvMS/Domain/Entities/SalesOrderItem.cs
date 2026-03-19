using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class SalesOrderItem
{
    public int Id { get; set; }

    public int SalesOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? LineTotal { get; set; }

    public virtual SalesOrder SalesOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
