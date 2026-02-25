using OrderManagement.Gateway.Application.DTOs.Provisioners.Token;
using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using System.Net.Http.Json;

namespace OrderManagement.Gateway.Persistence.Clients.Provisioner;

public sealed class ProvisionerApiMasterClient : IProvisionerApiMasterClient
{
    private readonly HttpClient _httpClient;

    public ProvisionerApiMasterClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient(HttpClientNames.ProvisionerApiMaster);
    }

    public async Task CreateUserAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PostAsJsonAsync("api/users", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task<TokenResponseDto> GetTokenAsync(TokenRequestDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PostAsJsonAsync("api/token/login", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<TokenResponseDto>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }
}
