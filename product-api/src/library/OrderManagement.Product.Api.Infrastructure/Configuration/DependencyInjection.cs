using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Product.Api.Application.Repositories;
using OrderManagement.Product.Api.Infrastructure.Configuration.Automapper;
using OrderManagement.Product.Api.Infrastructure.Repositories;

namespace OrderManagement.Product.Api.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static void AddProductApiPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Connection string 'MySql' not found.");

        services.AddDbContext<ProductDbContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
        });

        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddScoped<IProductRepository, ProductRepository>();
    }
}