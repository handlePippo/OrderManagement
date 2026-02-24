using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Category.Api.Application.Repositories;
using OrderManagement.Category.Api.Infrastructure.Configuration.Automapper;
using OrderManagement.Category.Api.Infrastructure.Repositories;

namespace OrderManagement.Category.Api.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static void AddCategoryApiPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Connection string 'MySql' not found.");

        services.AddDbContext<CategoryDbContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
        });

        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }
}