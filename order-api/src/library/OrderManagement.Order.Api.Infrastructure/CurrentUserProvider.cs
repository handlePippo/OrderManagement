using Microsoft.AspNetCore.Http;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Persistence.Extensions;

namespace OrderManagement.Order.Api.Persistence;

public sealed class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserProvider(IHttpContextAccessor http)
    {
        _http = http;
    }

    public int GetLoggedUserId() => _http?.HttpContext?.User?.GetLoggedUserId() ?? throw new InvalidOperationException("Invalid or missing User in HttpContext.");
    public string GetLoggedUsername() => _http?.HttpContext?.User?.GetLoggedUsername() ?? throw new InvalidOperationException("Invalid or missing User in HttpContext.");
}
