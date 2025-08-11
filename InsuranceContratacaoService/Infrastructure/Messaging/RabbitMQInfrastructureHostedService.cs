using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InsuranceContratacaoService.Infrastructure.Messaging
{
    public class RabbitMQInfrastructureHostedService : IHostedService
    {
        private readonly IRabbitMQInfrastructureService _infrastructureService;
        private readonly ILogger<RabbitMQInfrastructureHostedService> _logger;

        public RabbitMQInfrastructureHostedService(
            IRabbitMQInfrastructureService infrastructureService,
            ILogger<RabbitMQInfrastructureHostedService> logger)
        {
            _infrastructureService = infrastructureService;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting RabbitMQ infrastructure setup...");
                
                // Run infrastructure setup in a background task to avoid blocking startup
                await Task.Run(() => _infrastructureService.SetupInfrastructure(), cancellationToken);
                
                _logger.LogInformation("RabbitMQ infrastructure setup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to setup RabbitMQ infrastructure during startup");
                // Don't throw here to allow the application to continue starting
                // The services will handle missing infrastructure gracefully
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ infrastructure hosted service stopped");
            return Task.CompletedTask;
        }
    }
}
