namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Users;

public sealed record UpdateUserDto
{
    public string? Email { get; set; } = null!;
    public string? FirstName { get; set; } = null!;
    public string? LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; } = null!;
}