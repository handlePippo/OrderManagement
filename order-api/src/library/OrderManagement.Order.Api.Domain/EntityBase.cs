namespace OrderManagement.Order.Api.Domain;

public abstract class EntityBase
{
    /// <summary>
    /// Created at metadata.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Modified at metadata.
    /// </summary>
    public DateTime? ModifiedAt { get; protected set; }
}
