using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Provisioner.Api.Application.Automapper;
using OrderManagement.Provisioner.Api.Application.Interfaces;
using OrderManagement.Provisioner.Api.Application.Services;

namespace OrderManagement.Provisioner.Api.Application.Configuration;

public static class DependencyInjection
{
    public static void AddProvisionerServices(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<AutoMapperProfile>());

        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAddressService, AddressService>();
        services.AddTransient<ITokenService, TokenService>();
    }
}