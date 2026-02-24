using Microsoft.EntityFrameworkCore;
using OrderManagement.Category.Api.Application.Configuration;
using OrderManagement.Category.Api.Configuration;
using OrderManagement.Category.Api.Infrastructure.Configuration;

namespace OrderManagement.Category.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCategoryApiConfiguration(builder.Configuration);
            builder.Services.AddCategoryApiServices();
            builder.Services.AddCategoryApiPersistence(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CategoryDbContext>();
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
