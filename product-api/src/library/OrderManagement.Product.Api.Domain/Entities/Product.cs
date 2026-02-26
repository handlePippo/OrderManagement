namespace OrderManagement.Product.Api.Domain.Entities
{
    public sealed class Product
    {
        public int Id { get; private set; }
        public int CategoryId { get; private set; }
        public string Sku { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        /// <summary>
        /// Constructor for Automapper.
        /// </summary>
        private Product() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryId"></param>
        public Product(int id)
        {
            Id = id;
        }

        public void SetCategoryId(int categoryId) => CategoryId = categoryId;

        public void SetSku(string sku)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sku, nameof(sku));

            Sku = sku;
        }

        public void SetName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

            Name = name;
        }

        public void SetDescription(string? description) => Description = description;

        public void SetPrice(decimal price) => Price = price;

        public void MarkModified() => ModifiedAt = DateTime.UtcNow;

        public void IncreaseStock(int qty)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(qty);

            Stock += qty;
        }

        public void DecreaseStock(int qty)
        {
            if (Stock - qty <= 0)
            {
                ClearStock();
            }

            Stock -= qty;
        }

        public void SetStock(int stock) => Stock = stock;
        public void ClearStock() => Stock = 0;
    }
}