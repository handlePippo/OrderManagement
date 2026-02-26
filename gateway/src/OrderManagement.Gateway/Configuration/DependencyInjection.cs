using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagement.Gateway.Application.Interfaces;
using OrderManagement.Gateway.Application.Interfaces.Provisioner;
using OrderManagement.Gateway.Configuration.Middlewares;
using OrderManagement.Gateway.Infrastructure.Clients;
using OrderManagement.Gateway.Infrastructure.Clients.Category;
using OrderManagement.Gateway.Infrastructure.Clients.Order;
using OrderManagement.Gateway.Infrastructure.Clients.Product;
using OrderManagement.Gateway.Infrastructure.Clients.Provisioner;
using System.Security.Claims;

namespace OrderManagement.Gateway.Configuration
{
    public static class DependencyInjection
    {
        public static void AddGatewayConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Jwt configuration
            var jwtIssuer = configuration["Jwt:Issuer"];
            var jwtKey = configuration["Jwt:Key"];

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromHexString(jwtKey!)),
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            // SwaggerGen configuration
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Add the JWT token like that: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            services.AddMemoryCache();
            services.AddTransient<GlobalExceptionHandlingMiddleware>();
            services.AddTransient<AuthorizationForwardingHandler>();
            services.AddScoped<IProductApiClient, ProductApiClient>();
            services.AddScoped<ICategoryApiClient, CategoryApiClient>();
            services.AddScoped<IOrderApiClient, OrderApiClient>();
            services.AddScoped<IProvisionerApiAddressClient, ProvisionerApiAddressClient>();
            services.AddScoped<IProvisionerApiUserClient, ProvisionerApiUserClient>();
            services.AddScoped<IProvisionerApiMasterClient, ProvisionerApiMasterClient>();
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
                .AddHttpMessageHandler<AuthorizationForwardingHandler>()
                .AddStandardResilienceHandler();

            // category-api
            services.AddHttpClient(HttpClientNames.CategoryApi)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["CategoryApi:BaseUrl"]!);
                    c.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<AuthorizationForwardingHandler>()
                .AddStandardResilienceHandler();

            // order-api
            services.AddHttpClient(HttpClientNames.OrderApi)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["OrderApi:BaseUrl"]!);
                    c.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<AuthorizationForwardingHandler>()
                .AddStandardResilienceHandler();

            // provisioner-api
            services.AddHttpClient(HttpClientNames.ProvisionerApi)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["ProvisionerApi:BaseUrl"]!);
                    c.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddHttpMessageHandler<AuthorizationForwardingHandler>()
                .AddStandardResilienceHandler();

            // provisioner-api
            services.AddHttpClient(HttpClientNames.ProvisionerApiMaster)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration["ProvisionerApi:BaseUrl"]!);
                    c.Timeout = TimeSpan.FromSeconds(10);
                })
                .AddStandardResilienceHandler();
        }

        public static void ConfigureMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
}