namespace OrderManagement.Category.Api.Domain.Entities
{
    /// <summary>
    /// Category.
    /// </summary>
    public sealed class Category
    {
        /// <summary>
        /// Category id.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Catgory name.
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
        /// Constructor for Automapper.
        /// </summary>
        private Category() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        public Category(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Sets the category name.
        /// </summary>
        /// <param name="name"></param>
        public void SetName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            Name = name;
        }

        /// <summary>
        /// Marks the category modified.
        /// </summary>
        public void MarkModified() => ModifiedAt = DateTime.UtcNow;
    }
}