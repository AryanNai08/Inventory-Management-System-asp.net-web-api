using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class SalesOrder
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public int StatusId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();

    public virtual SalesOrderStatus Status { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
