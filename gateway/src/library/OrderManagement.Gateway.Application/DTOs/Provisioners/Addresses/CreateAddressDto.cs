using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses
{
    public sealed record CreateAddressDto
    {
        [Required]
        public required string CountryCode { get; set; }

        [Required]
        public required string City { get; set; }

        [Required]
        public required string PostalCode { get; set; }

        [Required]
        public required string Street { get; set; }
    }
}