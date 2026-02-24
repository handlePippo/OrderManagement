using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Category.Api.Application.Configuration.Automapper;
using OrderManagement.Category.Api.Application.Interfaces;
using OrderManagement.Category.Api.Application.Services;

namespace OrderManagement.Category.Api.Application.Configuration;

public static class DependencyInjection
{
    public static void AddCategoryApiServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddTransient<ICategoryService, CategoryService>();
    }
}