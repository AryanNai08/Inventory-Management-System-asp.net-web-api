using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PurchaseOrder
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int SupplierId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }

    public int StatusId { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual PurchaseOrderStatuses Status { get; set; } = null!;
    
    public virtual Supplier Supplier { get; set; } = null!;
}
