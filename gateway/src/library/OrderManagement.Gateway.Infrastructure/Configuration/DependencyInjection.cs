using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Gateway.Application.Interfaces;

namespace OrderManagement.Gateway.Persistence.Configuration;

public static class DependencyInjection
{
    public static void AddGatewayInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
    }
}