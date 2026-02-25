namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses
{
    public record AddressDto : EntityBaseDto
    {
        public int UserId { get; set; }
        public string CountryCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Street { get; set; } = null!;

        public AddressDto(int id) : base(id) { }
    }
}
