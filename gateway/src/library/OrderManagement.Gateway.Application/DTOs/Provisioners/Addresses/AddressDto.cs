namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses
{
    public record AddressDto : EntityBaseDto
    {
        public int UserId { get; init; }
        public string CountryCode { get; init; } = null!;
        public string City { get; init; } = null!;
        public string PostalCode { get; init; } = null!;
        public string Street { get; init; } = null!;

        public AddressDto(int id) : base(id) { }
    }
}
