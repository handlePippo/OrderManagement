using OrderManagement.Gateway.Configuration;
using OrderManagement.Gateway.Persistence.Configuration;

namespace OrderManagement.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGatewayConfiguration(builder.Configuration);
            builder.Services.AddGatewayInfrastructure(builder.Configuration);
            builder.Services.AddOrderApiHttpClients(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

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
