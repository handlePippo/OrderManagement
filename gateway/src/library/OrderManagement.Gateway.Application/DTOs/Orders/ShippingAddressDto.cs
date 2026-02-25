namespace OrderManagement.Gateway.Application.DTOs.Orders
{
    public sealed record ShippingAddressDto
    {
        public string ShipAddress { get; set; } = null!;
        public string ShipCity { get; set; } = null!;
        public string ShipPostalCode { get; set; } = null!;
        public string ShipCountryCode { get; set; } = null!;
        public string ShipPhoneNumber { get; set; } = null!;
    }
}