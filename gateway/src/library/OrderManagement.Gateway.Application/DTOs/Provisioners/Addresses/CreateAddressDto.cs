using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses
{
    public sealed record CreateAddressDto
    {
        [Required]
        public required string CountryCode { get; init; }

        [Required]
        public required string City { get; init; }

        [Required]
        public required string PostalCode { get; init; }

        [Required]
        public required string Street { get; init; }
    }
}