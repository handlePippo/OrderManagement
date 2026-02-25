namespace OrderManagement.Gateway.Application.DTOs.Provisioners.Token
{
    public sealed record TokenResponseDto
    {
        public string Jwt { get; set; } = null!;

        public TokenResponseDto(string jwt)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(jwt, nameof(jwt));

            Jwt = jwt;
        }
    }
}