namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Token
{
    public sealed record TokenRequestDto
    {
        public required string Email { get; set; } = null!;
        public required string Password { get; set; } = null!;
    }
}