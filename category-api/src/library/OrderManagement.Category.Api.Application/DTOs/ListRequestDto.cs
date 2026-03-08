using OrderManagement.Category.Api.Domain.Pagination;

namespace OrderManagement.Category.Api.Application.DTOs;

public sealed class ListRequestDto
{
    public int? Page { get; init; } = 1;
    public int? Size { get; init; } = 3;
    public PageOrder? Order { get; init; } = PageOrder.Asc;
}