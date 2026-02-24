using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Provisioner.Api.Application.DTOs.Users;

public sealed record CreateUserDto
{
    [Required]
    public required string Email { get; init; } = null!;

    [Required]
    public required string Password { get; init; } = null!;

    [Required]
    public required string FirstName { get; init; } = null!;

    [Required]
    public required string LastName { get; init; } = null!;

    [Required]
    public required string PhoneNumber { get; init; } = null!;
}