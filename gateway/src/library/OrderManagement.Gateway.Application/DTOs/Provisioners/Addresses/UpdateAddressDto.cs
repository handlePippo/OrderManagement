namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses
{
    public sealed record UpdateAddressDto
    {
        public string? CountryCode { get; init; } = null!;
        public string? City { get; init; } = null!;
        public string? PostalCode { get; init; } = null!;
        public string? Street { get; init; } = null!;
    }
}