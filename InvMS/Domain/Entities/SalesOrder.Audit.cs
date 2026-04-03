using Domain.Common;

namespace Domain.Entities;

// SalesOrders have no soft delete, so they use IAuditable (not ISoftDeletable).
// No DeletedBy field needed here.
public partial class SalesOrder : IAuditable
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
