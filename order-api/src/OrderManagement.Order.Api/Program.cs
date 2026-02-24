using Microsoft.EntityFrameworkCore;
using OrderManagement.Order.Api.Application.Configuration;
using OrderManagement.Order.Api.Configuration;
using OrderManagement.Order.Api.Persistence.Configuration;

namespace OrderManagement.Order.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddOrderApiConfiguration(builder.Configuration);
            builder.Services.AddOrderApiServices();
            builder.Services.AddOrderApiPersistence(builder.Configuration);
            builder.Services.AddOrderApiHttpClients(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                db.Database.Migrate();
            }

            app.ConfigureMiddlewares();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
