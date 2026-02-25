namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Token
{
    public sealed record TokenResponseDto
    {
        public string Jwt { get; set; } = null!;
    }
}