namespace OrderManagement.Order.Api.Domain.Entities;

public sealed class User : EntityBase
{
    public int Id { get; set; }
    public required string Email { get; init; } = null!;
    public required string FirstName { get; init; } = null!;
    public required string LastName { get; init; } = null!;
    public required string PhoneNumber { get; init; } = null!;
}