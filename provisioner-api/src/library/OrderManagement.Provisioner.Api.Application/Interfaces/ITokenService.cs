using OrderManagement.Provisioner.Api.Application.DTOs.Token;

namespace OrderManagement.Provisioner.Api.Application.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponseDto> GetTokenAsync(TokenRequestDto loginRequest, CancellationToken cancellationToken);
    }
}