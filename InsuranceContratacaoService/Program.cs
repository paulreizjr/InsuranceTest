using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceContratacaoService.Infrastructure.Messaging;
using InsuranceContratacaoService.Services;
using InsuranceContratacaoService.DTOs;

namespace InsuranceContratacaoService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<RabbitMQSettings>(
                builder.Configuration.GetSection("RabbitMQ"));

            // Register RabbitMQ infrastructure services
            builder.Services.AddSingleton<IRabbitMQInfrastructureService, RabbitMQInfrastructureService>();
            builder.Services.AddHostedService<RabbitMQInfrastructureHostedService>();

            builder.Services.AddSingleton<IContratoPropostaMessagePublisher, ContratoPropostaMessagePublisher>();

            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
