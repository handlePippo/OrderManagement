using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Order.Api.Application.DTOs.Orders.Create;

public sealed record CreateShippingAddressDto
{
    [Required]
    public required string ShipAddress { get; init; } = null!;

    [Required]
    public required string ShipCity { get; init; } = null!;

    [Required]
    public required string ShipPostalCode { get; init; } = null!;

    [Required]
    public required string ShipCountryCode { get; init; } = null!;

    [Required]
    public required string ShipPhoneNumber { get; init; } = null!;
}
