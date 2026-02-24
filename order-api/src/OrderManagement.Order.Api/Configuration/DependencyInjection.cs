using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagement.Order.Api.Application.Interfaces;
using OrderManagement.Order.Api.Configuration.Middlewares;
using OrderManagement.Order.Api.Persistence.Clients;

namespace OrderManagement.Order.Api.Configuration
{
    public static class DependencyInjection
    {
        public static void AddOrderApiConfiguration(this IServiceCollection services, IConfiguration configuration)
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
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromHexString(jwtKey!))
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

            services.AddTransient<GlobalExceptionHandlingMiddleware>();
            services.AddScoped<ValidateAuthorizationFilter>();
        }

        public static void AddOrderApiHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IProductApiClient, ProductApiClient>();
            services.AddHttpClient<IProductApiClient, ProductApiClient>(c =>
            {
                c.BaseAddress = new Uri(configuration["ProductApi:BaseUrl"]!);
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