using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Order.Api.Application.Configuration.Automapper;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Application.Services;

namespace OrderManagement.Order.Api.Application.Configuration;

public static class DependencyInjection
{
    public static void AddOrderApiServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<IOrderNormalizerService, OrderNormalizerService>();
    }
}