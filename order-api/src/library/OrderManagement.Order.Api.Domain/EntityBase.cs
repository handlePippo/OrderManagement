namespace OrderManagement.Order.Api.Domain;

public abstract class EntityBase
{
    /// <summary>
    /// Id of the order.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Created at metadata.
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Modified at metadata.
    /// </summary>
    public DateTime? ModifiedAt { get; protected set; }

    /// <summary>
    /// Constructor for EF / Automapper.
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
