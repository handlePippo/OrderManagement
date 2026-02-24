using Microsoft.EntityFrameworkCore;
using OrderManagement.Product.Api.Application.Configuration;
using OrderManagement.Product.Api.Configuration;
using OrderManagement.Product.Api.Infrastructure.Configuration;

namespace OrderManagement.Product.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProductApiConfiguration(builder.Configuration);
            builder.Services.AddProductApiServices();
            builder.Services.AddProductApiPersistence(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
                db.Database.Migrate();
            }

            app.ConfigureMiddlewares();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
