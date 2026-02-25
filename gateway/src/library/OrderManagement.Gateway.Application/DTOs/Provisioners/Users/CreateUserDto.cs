using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Users;

public sealed record CreateUserDto
{
    [Required]
    public required string Email { get; set; } = null!;

    [Required]
    public required string Password { get; set; } = null!;

    [Required]
    public required string FirstName { get; set; } = null!;

    [Required]
    public required string LastName { get; set; } = null!;

    [Required]
    public required string PhoneNumber { get; set; } = null!;
}