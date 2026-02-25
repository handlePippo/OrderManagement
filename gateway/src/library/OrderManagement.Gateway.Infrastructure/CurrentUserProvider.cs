using Microsoft.AspNetCore.Http;
using OrderManagement.Gateway.Application.Interfaces;
using OrderManagement.Gateway.Persistence.Extensions;

namespace OrderManagement.Gateway.Persistence;

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
