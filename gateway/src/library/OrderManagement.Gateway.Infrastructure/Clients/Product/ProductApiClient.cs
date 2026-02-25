using OrderManagement.Gateway.Application.DTOs.Products;
using OrderManagement.Gateway.Application.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.Gateway.Persistence.Clients.Product;

public sealed class ProductApiClient : IProductApiClient
{
    private readonly HttpClient _httpClient;

    public ProductApiClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient(HttpClientNames.ProductApi);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/products/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/products/list", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<ProductDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/products/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<IReadOnlyList<ProductDto>> GetRangeAsync(GetProductRangeDto dto, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products/range", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<ProductDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PostAsJsonAsync("api/products", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }
}
