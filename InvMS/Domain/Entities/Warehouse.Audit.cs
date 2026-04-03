using Domain.Common;

namespace Domain.Entities;

public partial class Warehouse : ISoftDeletable
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
}
