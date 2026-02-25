namespace OrderManagement.Order.Api.Domain.Entities
{
    public sealed class ShippingAddress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ShipAddress { get; set; } = null!;
        public string ShipCity { get; set; } = null!;
        public string ShipPostalCode { get; set; } = null!;
        public string ShipCountryCode { get; set; } = null!;
        public string ShipPhoneNumber { get; set; } = null!;
    }
}