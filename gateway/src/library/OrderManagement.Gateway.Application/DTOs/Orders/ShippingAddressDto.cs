namespace OrderManagement.Gateway.Application.DTOs.Orders
{
    public sealed record ShippingAddressDto
    {
        public string ShipAddress { get; private set; } = null!;
        public string ShipCity { get; private set; } = null!;
        public string ShipPostalCode { get; private set; } = null!;
        public string ShipCountryCode { get; private set; } = null!;
        public string ShipPhoneNumber { get; private set; } = null!;
    }
}