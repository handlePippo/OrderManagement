using Microsoft.AspNetCore.Authentication;
using OrderManagement.Product.Api.Configuration.Middlewares;

namespace OrderManagement.Product.Api.Configuration
{
    public static class DependencyInjection
    {
        public static void AddProductApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(GatewayHeaderAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, GatewayHeaderAuthHandler>(GatewayHeaderAuthHandler.SchemeName, _ => { });

            services.AddAuthorization();
            services.AddTransient<GlobalExceptionHandlingMiddleware>();
        }

        public static void ConfigureMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}