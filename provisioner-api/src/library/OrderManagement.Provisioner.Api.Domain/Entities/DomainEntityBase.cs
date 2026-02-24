namespace OrderManagement.Provisioner.Api.Domain.Entities;

/// <summary>
/// Base domain entity.
/// </summary>
public abstract class DomainEntityBase
{
    /// <summary>
    /// Id.
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
    /// Marks the user modified.
    /// </summary>
    public void MarkModified() => ModifiedAt = DateTime.UtcNow;

    /// <summary>
    /// Constructor for Automapper.
    /// </summary>
    protected DomainEntityBase() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    protected DomainEntityBase(int id)
    {
        Id = id;
    }
}
