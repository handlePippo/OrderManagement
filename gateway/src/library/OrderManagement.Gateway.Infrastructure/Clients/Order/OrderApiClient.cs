using OrderManagement.Gateway.Application.DTOs.Orders;
using OrderManagement.Gateway.Application.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.Gateway.Persistence.Clients.Order;

public sealed class OrderApiClient : IOrderApiClient
{
    private readonly HttpClient _httpClient;

    public OrderApiClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient(HttpClientNames.OrderApi);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/orders/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Order API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
    }

    public async Task<IReadOnlyList<OrderDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/orders/list", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Order API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<OrderDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<OrderDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/orders/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Order API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task CreateAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PostAsJsonAsync("api/orders", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Order API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task UpdateAsync(Guid id, UpdateOrderDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Order API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/orders/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Order API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }
}