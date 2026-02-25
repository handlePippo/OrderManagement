using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Repositories;
using OrderManagement.Provisioner.Api.Persistence.Configuration.Automapper;
using OrderManagement.Provisioner.Api.Persistence.Repositories;

namespace OrderManagement.Provisioner.Api.Persistence.Configuration;

public static class DependencyInjection
{
    public static void AddProvisionerPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySql")
            ?? throw new InvalidOperationException("Connection string 'MySql' not found");

        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
        });

        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
    }
}