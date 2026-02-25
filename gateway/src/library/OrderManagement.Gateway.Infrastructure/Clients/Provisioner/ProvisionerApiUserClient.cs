using OrderManagement.Gateway.Application.DTOs.Provisioners.Users;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.Gateway.Persistence.Clients.Provisioner;

public sealed class ProvisionerApiUserClient : IProvisionerApiUserClient
{
    private readonly HttpClient _httpClient;

    public ProvisionerApiUserClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient(HttpClientNames.ProvisionerApi);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/users/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
    }

    public async Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/users/list", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<UserDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<UserDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/users/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task UpdateAsync(int id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PutAsJsonAsync($"api/users/{id}", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/users/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }
}
