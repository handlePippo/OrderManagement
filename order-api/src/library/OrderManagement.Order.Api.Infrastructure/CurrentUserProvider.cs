using Microsoft.AspNetCore.Http;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Infrastructure.Extensions;

namespace OrderManagement.Order.Api.Infrastructure;

public sealed class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserProvider(IHttpContextAccessor http)
    {
        _http = http;
    }

    public bool IsAdmin => _http?.HttpContext?.User?.IsAdmin() ?? throw new InvalidOperationException("Invalid or missing User in HttpContext.");
    public int GetLoggedUserId() => _http?.HttpContext?.User?.GetLoggedUserId() ?? throw new InvalidOperationException("Invalid or missing User in HttpContext.");
}
