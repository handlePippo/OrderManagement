namespace OrderManagement.Product.Api.Domain.Entities
{
    public record ProductRange
    {
        public IReadOnlyList<int> Ids { get; set; }

        public ProductRange(IReadOnlyList<int> orderIds)
        {
            Ids = orderIds ?? throw new ArgumentNullException(nameof(orderIds));
        }
    }
}