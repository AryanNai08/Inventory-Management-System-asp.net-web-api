using Domain.Common;

namespace Domain.Entities;

public partial class Category : ISoftDeletable
{
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string? DeletedBy { get; set; }
}
