using Microsoft.AspNetCore.Http;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Infrastructure.Extensions;

namespace OrderManagement.Provisioner.Api.Infrastructure;

public sealed class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserProvider(IHttpContextAccessor http)
    {
        _http = http;
    }

    public bool IsAdmin => _http?.HttpContext?.User?.IsAdmin() 
        ?? throw new InvalidOperationException("Invalid or missing User in HttpContext.");

    public int GetLoggedUserId() => _http?.HttpContext?.User?.GetLoggedUserId()
        ?? throw new InvalidOperationException("Invalid or missing User in HttpContext.");
}