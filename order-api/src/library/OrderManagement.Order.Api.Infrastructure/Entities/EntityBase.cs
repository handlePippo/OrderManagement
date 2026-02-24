namespace OrderManagement.Order.Api.Persistence.Entities;

/// <summary>
/// Base entity used to share common properties (e.g id, metadata).
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Entity id.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Created at metadata.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Modified at metadata.
    /// </summary>
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    protected EntityBase() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    protected EntityBase(int id)
    {
        Id = id;
    }
}