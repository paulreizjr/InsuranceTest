using InsuranceContratacaoService.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace InsurancePropostaService.Infrastructure.Messaging
{
    public interface IRabbitMQInfrastructureService
    {
        void SetupInfrastructure();
    }

    public class RabbitMQInfrastructureService : IRabbitMQInfrastructureService, IDisposable
    {
        private readonly RabbitMQSettings _settings;
        private readonly ILogger<RabbitMQInfrastructureService> _logger;
        private IConnection? _connection;
        private IModel? _channel;

        public RabbitMQInfrastructureService(
            IOptions<RabbitMQSettings> settings,
            ILogger<RabbitMQInfrastructureService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public void SetupInfrastructure()
        {
            try
            {
                _logger.LogInformation("Setting up RabbitMQ infrastructure...");

                var factory = new ConnectionFactory()
                {
                    Uri = new Uri(_settings.ConnectionString)
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                // Declare exchange
                _channel.ExchangeDeclare(_settings.ExchangeName, ExchangeType.Direct, durable: true);
                _logger.LogInformation($"Exchange '{_settings.ExchangeName}' declared");

                // Declare dead letter queue
                _channel.QueueDeclare(_settings.DeadLetterQueue, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(_settings.DeadLetterQueue, _settings.ExchangeName, _settings.DeadLetterQueue);
                _logger.LogInformation($"Dead letter queue '{_settings.DeadLetterQueue}' declared and bound");

                // Configure main queue arguments with DLQ and TTL
                var queueArgs = new Dictionary<string, object>
                {
                    {"x-dead-letter-exchange", _settings.ExchangeName},
                    {"x-dead-letter-routing-key", _settings.DeadLetterQueue},
                    {"x-message-ttl", _settings.MessageTtlMs}
                };

                // Declare proposta status request queue with DLQ configuration
                _channel.QueueDeclare(_settings.PropostaStatusRequestQueue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
                _channel.QueueBind(_settings.PropostaStatusRequestQueue, _settings.ExchangeName, "proposta.status.request");
                _logger.LogInformation($"Proposta status request queue '{_settings.PropostaStatusRequestQueue}' declared and bound");

                // Declare proposta status response queue
                _channel.QueueDeclare(_settings.PropostaStatusResponseQueue, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(_settings.PropostaStatusResponseQueue, _settings.ExchangeName, "proposta.status.response");
                _logger.LogInformation($"Proposta status response queue '{_settings.PropostaStatusResponseQueue}' declared and bound");

                // Declare gera contrato request queue with DLQ configuration
                _channel.QueueDeclare(_settings.GeraContratoRequestQueue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
                _channel.QueueBind(_settings.GeraContratoRequestQueue, _settings.ExchangeName, "gera.contrato.request");
                _logger.LogInformation($"Gera contrato request queue '{_settings.GeraContratoRequestQueue}' declared and bound");

                // Declare gera contrato response queue
                _channel.QueueDeclare(_settings.GeraContratoResponseQueue, durable: true, exclusive: false, autoDelete: false);
                _channel.QueueBind(_settings.GeraContratoResponseQueue, _settings.ExchangeName, "gera.contrato.response");
                _logger.LogInformation($"Gera contrato response queue '{_settings.GeraContratoResponseQueue}' declared and bound");

                _logger.LogInformation("RabbitMQ infrastructure setup completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to setup RabbitMQ infrastructure");
                throw;
            }
            finally
            {
                // Close the setup connection - services will create their own connections
                _channel?.Close();
                _connection?.Close();
                _channel?.Dispose();
                _connection?.Dispose();
                _channel = null;
                _connection = null;
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
