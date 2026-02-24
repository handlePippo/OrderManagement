namespace OrderManagement.Provisioner.Api.Application.DTOs.Users;

public sealed record UpdateUserDto
{
    public string? Email { get; init; } = null!;
    public string? FirstName { get; init; } = null!;
    public string? LastName { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
}