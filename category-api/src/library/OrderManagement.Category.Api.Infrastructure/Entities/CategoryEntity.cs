namespace OrderManagement.Category.Api.Infrastructure.Entities;

/// <summary>
/// Category entity.
/// </summary>
public sealed class CategoryEntity
{
    /// <summary>
    /// Category id.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Category name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Created at metadata.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Modified at metadata.
    /// </summary>
    public DateTime? ModifiedAt { get; private set; }

    /// <summary>
    /// Constructor for EF / Automapper.
    /// </summary>
    private CategoryEntity() { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public CategoryEntity(int id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name;
    }
}