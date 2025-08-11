
using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Application.UseCases;
using InsurancePropostaService.Infrastructure.Messaging;
using InsurancePropostaService.Data;
using InsurancePropostaService.Repositories;
using InsurancePropostaService.Services;
using Microsoft.EntityFrameworkCore;
using InsuranceCoreBusiness.Application.Services;
using InsuranceContratacaoService.DTOs;

namespace InsurancePropostaService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            // Add DbContext
            builder.Services.AddDbContext<InsuranceDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register services
            builder.Services.AddScoped<IPropostaService, PropostaService>();

            // Register repositories
            builder.Services.AddScoped<IContratoPropostaRepository, ContratoPropostaRepository>();
            builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();

            // Register use cases
            builder.Services.AddScoped<ICrudContratoPropostaUC, CrudContratoPropostaUC>();
            builder.Services.AddScoped<ICrudPropostaUC, CrudPropostaUC>();
            builder.Services.AddScoped<IOperacoesContratoUC, OperacoesContratoUC>();
            builder.Services.AddScoped<IOperacoesPropostaUC, OperacoesPropostaUC>();

            // Configure RabbitMQ settings
            builder.Services.Configure<RabbitMQSettings>(
                builder.Configuration.GetSection("RabbitMQ"));

            // Register RabbitMQ infrastructure services
            builder.Services.AddSingleton<IRabbitMQInfrastructureService, RabbitMQInfrastructureService>();
            builder.Services.AddHostedService<RabbitMQInfrastructureHostedService>();

            // Register RabbitMQ consumer services
            builder.Services.AddHostedService<PropostaStatusConsumerService>();
            builder.Services.AddHostedService<GeraContratoConsumerService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Run migrations automatically in Development environment
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
                    try
                    {
                        dbContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred while migrating the database.");
                    }
                }
            }

            // Configure the HTTP request pipeline.
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
