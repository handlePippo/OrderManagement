namespace OrderManagement.Order.Api.Infrastructure.Entities;

/// <summary>
/// Base entity used to share common properties (e.g id, metadata).
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Created at metadata.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Modified at metadata.
    /// </summary>
    public DateTime? ModifiedAt { get; private set; }
}