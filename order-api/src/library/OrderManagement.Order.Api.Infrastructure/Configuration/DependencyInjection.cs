using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Repositories;
using OrderManagement.Order.Api.Infrastructure.Configuration.Automapper;
using OrderManagement.Order.Api.Infrastructure.Repositories;

namespace OrderManagement.Order.Api.Infrastructure.Configuration;

public static class DependencyInjection
{
    private const string ProvisioningName = "MySql";

    public static void AddOrderApiPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ProvisioningName)
            ?? throw new InvalidOperationException($"Connection string '{ProvisioningName}' not found.");

        services.AddDbContext<OrderDbContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
        });

        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}