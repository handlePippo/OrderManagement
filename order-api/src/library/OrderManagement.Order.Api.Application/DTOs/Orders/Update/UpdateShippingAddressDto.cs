namespace OrderManagement.Order.Api.Application.DTOs.Orders.Update;

public sealed record UpdateShippingAddressDto
{
    public string? ShipAddress { get; init; } = null!;
    public string? ShipCity { get; init; } = null!;
    public string? ShipPostalCode { get; init; } = null!;
    public string? ShipCountryCode { get; init; } = null!;
    public string? ShipPhoneNumber { get; init; } = null!;
}