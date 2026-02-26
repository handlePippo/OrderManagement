using OrderManagement.Gateway.Application.DTOs.Provisioners.Addresses;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.Gateway.Infrastructure.Clients.Provisioner;

public sealed class ProvisionerApiAddressClient : IProvisionerApiAddressClient
{
    private readonly HttpClient _httpClient;

    public ProvisionerApiAddressClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient(HttpClientNames.ProvisionerApi);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/addresses/{id}", cancellationToken);

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

    public async Task<IReadOnlyList<AddressDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/addresses/list", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<AddressDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<AddressDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/addresses/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<AddressDto>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task CreateAsync(CreateAddressDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PostAsJsonAsync("api/addresses", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task UpdateAsync(int id, UpdateAddressDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PutAsJsonAsync($"api/addresses/{id}", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/addresses/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Provisioner API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }
}
