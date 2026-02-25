using Microsoft.AspNetCore.Authentication;
using OrderManagement.Provisioner.Api.Configuration.Middlewares;

namespace OrderManagement.Provisioner.Api.Configuration
{
    public static class DependencyInjection
    {
        public static void AddProvisionerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(GatewayHeaderAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, GatewayHeaderAuthHandler>(GatewayHeaderAuthHandler.SchemeName, _ => { });

            services.AddAuthorization();
            services.AddMemoryCache();
            services.AddTransient<GlobalExceptionHandlingMiddleware>();
            services.AddScoped<ValidateUserAuthorizationFilter>();
            services.AddScoped<ValidateAddressAuthorizationFilter>();
        }

        public static void ConfigureMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}