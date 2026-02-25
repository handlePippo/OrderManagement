using OrderManagement.Gateway.Application.DTOs.Categories;
using OrderManagement.Gateway.Application.Interfaces;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.Gateway.Persistence.Clients.Category;

public sealed class CategoryApiClient : ICategoryApiClient
{
    private readonly HttpClient _httpClient;

    public CategoryApiClient(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient(HttpClientNames.CategoryApi);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/categories/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Category API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<bool>(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("api/categories/list", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Category API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<CategoryDto>>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task<CategoryDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/categories/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Category API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response.");
    }

    public async Task CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PostAsJsonAsync("api/categories", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Category API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var response = await _httpClient.PutAsJsonAsync($"api/categories/{id}", dto, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Category API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/categories/{id}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Category API failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }
    }
}