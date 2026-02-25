using OrderManagement.Gateway.Application.DTOs.Provisioners.Token;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;

namespace OrderManagement.Gateway.Application.Interfaces.Provisioner;

public interface IProvisionerApiMasterClient
{
    Task CreateUserAsync(CreateUserDto entity, CancellationToken cancellationToken = default);
    Task<TokenResponseDto> GetTokenAsync(TokenRequestDto loginRequest, CancellationToken cancellationToken);
}
