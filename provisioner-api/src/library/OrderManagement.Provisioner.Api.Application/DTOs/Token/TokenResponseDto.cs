namespace OrderManagement.Provisioner.Api.Application.DTOs.Token
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