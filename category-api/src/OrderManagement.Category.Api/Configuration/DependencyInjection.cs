using Microsoft.AspNetCore.Authentication;
using OrderManagement.Category.Api.Configuration.Middlewares;

namespace OrderManagement.Category.Api.Configuration
{
    public static class DependencyInjection
    {
        public static void AddCategoryApiConfiguration(this IServiceCollection services, IConfiguration configuration)
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