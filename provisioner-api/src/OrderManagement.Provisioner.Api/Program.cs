using Microsoft.EntityFrameworkCore;
using OrderManagement.Provisioner.Api.Application.Configuration;
using OrderManagement.Provisioner.Api.Configuration;
using OrderManagement.Provisioner.Api.Persistence.Configuration;

namespace OrderManagement.Provisioner.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProvisionerConfiguration(builder.Configuration);
            builder.Services.AddProvisionerServices();
            builder.Services.AddProvisionerPersistence(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
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
