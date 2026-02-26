using Microsoft.EntityFrameworkCore;
using OrderManagement.Provisioner.Api.Application.Configuration;
using OrderManagement.Provisioner.Api.Configuration;
using OrderManagement.Provisioner.Api.Infrastructure.Configuration;

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
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            if(args.Contains("--migrate-only"))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                    db.Database.Migrate();
                }

                return;
            }

            app.ConfigureMiddlewares();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
