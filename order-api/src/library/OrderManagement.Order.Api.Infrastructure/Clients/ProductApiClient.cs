using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Domain.Entities;
using System.Net.Http.Json;

namespace OrderManagement.Order.Api.Persistence.Clients;

public sealed class ProductApiClient : IProductApiClient
{
    private readonly HttpClient _httpClient;

    public ProductApiClient(HttpClient http)
    {
        _httpClient = http;
    }

    public async Task<IReadOnlyList<Product>> GetRangeAsync(IReadOnlyList<int> ids, CancellationToken ct)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products/range", ids, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<List<ApiProduct>>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Failed to deserialize response.");

        return result.Select(p => new Product()
        {
            Id = p.Id,
            CategoryId = p.CategoryId,
            Name = p.Name,
            Description = p.Description,
            Sku = p.Sku,
            Price = p.Price
        }).ToList()!
        .AsReadOnly();
    }
}
