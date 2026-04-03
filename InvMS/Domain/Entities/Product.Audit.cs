using Domain.Common;

namespace Domain.Entities;

// This partial file is SAFE from re-scaffolding.
// It adds the audit fields to the auto-generated Product.cs.
public partial class Product : ISoftDeletable
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
}
