using Microsoft.AspNetCore.Authentication;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Configuration.Middlewares;
using OrderManagement.Order.Api.Persistence.Clients;
using OrderManagement.Order.Api.Persistence.Clients.Product;
using OrderManagement.Order.Api.Persistence.Clients.Provisioner;

namespace OrderManagement.Order.Api.Configuration
{
    public static class DependencyInjection
    {
        public static void AddOrderApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddAuthentication(GatewayHeaderAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, GatewayHeaderAuthHandler>(GatewayHeaderAuthHandler.SchemeName, _ => { });

            services.AddAuthorization();
            services.AddMemoryCache();
            services.AddTransient<GlobalExceptionHandlingMiddleware>();
            services.AddScoped<HeadersForwardingHandler>();
            services.AddScoped<ValidateAuthorizationFilter>();
            services.AddScoped<IProductApiClient, ProductApiClient>();
            services.AddScoped<IProvisionerApiClient, ProvisionerApiClient>();
        }

        public static void AddOrderApiHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            // product-api
            services.AddHttpClient(HttpClientNames.ProductApi)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["ProductApi:BaseUrl"]!);
                    c.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<HeadersForwardingHandler>()
                .AddStandardResilienceHandler();

            // provisioner-api
            services.AddHttpClient(HttpClientNames.ProvisionerApi)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["ProvisionerApi:BaseUrl"]!);
                    c.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<HeadersForwardingHandler>()
                .AddStandardResilienceHandler();
        }

        public static void ConfigureMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}