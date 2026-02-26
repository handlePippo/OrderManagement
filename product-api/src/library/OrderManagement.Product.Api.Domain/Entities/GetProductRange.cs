namespace OrderManagement.Product.Api.Domain.Entities
{
    public record GetProductRange
    {
        public IReadOnlyList<int> OrderIds { get; set; }

        public GetProductRange(IReadOnlyList<int> orderIds)
        {
            OrderIds = orderIds ?? throw new ArgumentNullException(nameof(orderIds));
        }
    }
}