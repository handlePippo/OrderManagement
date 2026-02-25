namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses
{
    public sealed record UpdateAddressDto
    {
        public string? CountryCode { get; set; } = null!;
        public string? City { get; set; } = null!;
        public string? PostalCode { get; set; } = null!;
        public string? Street { get; set; } = null!;
    }
}