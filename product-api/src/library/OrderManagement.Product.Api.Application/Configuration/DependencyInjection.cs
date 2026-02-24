using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Product.Api.Application.Configuration.Automapper;
using OrderManagement.Product.Api.Application.Interfaces;
using OrderManagement.Product.Api.Application.Services;

namespace OrderManagement.Product.Api.Application.Configuration;

public static class DependencyInjection
{
    public static void AddProductApiServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddTransient<IProductService, ProductService>();
    }
}