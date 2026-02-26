using AutoMapper;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Domain.ValueObjects;
using System.Net.Http.Json;

namespace OrderManagement.Order.Api.Infrastructure.Clients.Product;

public sealed class ProductApiClient : IProductApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IMapper _mapper;

    public ProductApiClient(IHttpClientFactory factory, IMapper mapper)
    {
        _httpClient = factory.CreateClient(HttpClientNames.ProductApi);
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<Domain.Entities.Product>> GetProductsAsync(ProductRange range, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(range);

        var response = await _httpClient.PostAsJsonAsync("api/products/range", range, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            throw new HttpRequestException($"Product API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<List<ApiProduct>>(cancellationToken: ct)
            ?? throw new InvalidOperationException("Failed to deserialize response.");

        return result
            .Select(_mapper.Map<Domain.Entities.Product>)
            .ToList()!
            .AsReadOnly();
    }
}
