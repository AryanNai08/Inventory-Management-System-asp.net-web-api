namespace Domain.Common;

/// <summary>
/// Marks an entity as auditable.
/// The DbContext uses this interface to automatically populate
/// CreatedBy, UpdatedBy, and DeletedBy when SaveChangesAsync is called.
/// </summary>
public interface IAuditable
{
    string? CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }

    string? UpdatedBy { get; set; }
    DateTime? ModifiedDate { get; set; }

    // Only entities with soft delete will have DeletedBy
}

/// <summary>
/// Extended interface for entities that support soft delete tracking.
/// </summary>
public interface ISoftDeletable : IAuditable
{
    bool IsDeleted { get; set; }
    string? DeletedBy { get; set; }
}
