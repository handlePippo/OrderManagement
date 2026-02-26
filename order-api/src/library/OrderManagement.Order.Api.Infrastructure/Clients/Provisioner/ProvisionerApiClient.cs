using AutoMapper;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Domain.Entities;
using OrderManagement.Order.Api.Infrastructure.Clients;
using System.Net.Http.Json;

namespace OrderManagement.Order.Api.Infrastructure.Clients.Provisioner;

public sealed class ProvisionerApiClient : IProvisionerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public ProvisionerApiClient(IHttpClientFactory factory, IMapper mapper)
    {
        _httpClient = factory.CreateClient(HttpClientNames.ProvisionerApi);
        _mapper = mapper;
    }

    public async Task<ShippingAddress> GetShippingAddressAsync(int id, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"api/addresses/{id}", ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        var apiProduct = await response.Content.ReadFromJsonAsync<ApiShippingAddress>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Failed to deserialize response.");

        return _mapper.Map<ShippingAddress>(apiProduct);
    }

    public async Task<User> GetUserAsync(int id, CancellationToken ct)
    {
        var response = await _httpClient.GetAsync($"api/users/{id}", ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        var apiUser = await response.Content.ReadFromJsonAsync<ApiUser>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Failed to deserialize response.");

        return _mapper.Map<User>(apiUser);
    }
}
