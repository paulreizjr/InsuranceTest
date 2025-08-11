using InsuranceContratacaoService.DTOs;
using InsuranceCoreBusiness.Application.Ports.Outbound;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace InsurancePropostaService.Services
{
    public class PropostaStatusConsumerService : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PropostaStatusConsumerService> _logger;

        public PropostaStatusConsumerService(
            IOptions<RabbitMQSettings> settings,
            IServiceProvider serviceProvider,
            ILogger<PropostaStatusConsumerService> logger)
        {
            _settings = settings.Value;
            _serviceProvider = serviceProvider;
            _logger = logger;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_settings.ConnectionString)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Infrastructure setup is handled by RabbitMQInfrastructureService
            // This consumer only connects to existing queues
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, e) =>
            {
                var retryCount = GetRetryCount(e.BasicProperties);
                
                try
                {
                    await ProcessMessage(e);
                    _channel.BasicAck(e.DeliveryTag, false);
                    _logger.LogDebug($"Message processed successfully (retry: {retryCount})");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing proposta status message (attempt {retryCount + 1}/{_settings.MaxRetries + 1})");
                    
                    if (retryCount < _settings.MaxRetries)
                    {
                        // Requeue with retry count
                        await RequeueWithRetry(e, retryCount + 1, ex);
                        _channel.BasicAck(e.DeliveryTag, false); // Acknowledge original message
                    }
                    else
                    {
                        // Max retries exceeded, send to dead letter queue
                        _logger.LogError($"Max retries exceeded for message. Sending to DLQ: {ex.Message}");
                        _channel.BasicNack(e.DeliveryTag, false, false); // Don't requeue, will go to DLQ
                    }
                }
            };

            _channel.BasicConsume(_settings.PropostaStatusRequestQueue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task ProcessMessage(BasicDeliverEventArgs e)
        {
            var requestJson = Encoding.UTF8.GetString(e.Body.ToArray());
            var request = JsonSerializer.Deserialize<PropostaStatusRequestDto>(requestJson);

            if (request == null)
            {
                _logger.LogWarning("Received invalid request message");
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var propostaRepository = scope.ServiceProvider.GetRequiredService<IPropostaRepository>();

            var proposta = await propostaRepository.GetByIdAsync(request.PropostaId);

            var response = new PropostaStatusResponseDto
            {
                PropostaId = request.PropostaId,
                RequestId = request.RequestId,
                PropostaFound = proposta != null,
                StatusProposta = proposta?.statusProposta ?? default
            };

            var responseJson = JsonSerializer.Serialize(response);
            var responseBody = Encoding.UTF8.GetBytes(responseJson);

            var replyProps = _channel.CreateBasicProperties();
            replyProps.CorrelationId = e.BasicProperties.CorrelationId;

            // Send response to the reply queue specified in the request
            if (!string.IsNullOrEmpty(e.BasicProperties.ReplyTo))
            {
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: e.BasicProperties.ReplyTo,
                    basicProperties: replyProps,
                    body: responseBody);
            }

            _logger.LogInformation($"Processed status request for proposta {request.PropostaId}");
        }

        private int GetRetryCount(IBasicProperties properties)
        {
            if (properties?.Headers?.ContainsKey("retry-count") == true)
            {
                return Convert.ToInt32(properties.Headers["retry-count"]);
            }
            return 0;
        }

        private async Task RequeueWithRetry(BasicDeliverEventArgs e, int retryCount, Exception lastException)
        {
            try
            {
                // Calculate delay with exponential backoff (2^retryCount seconds)
                var delaySeconds = Math.Pow(2, retryCount);
                _logger.LogWarning($"Requeuing message with retry count {retryCount} after {delaySeconds} seconds delay. Last error: {lastException.Message}");
                
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));

                var props = _channel.CreateBasicProperties();
                props.CorrelationId = e.BasicProperties.CorrelationId;
                props.ReplyTo = e.BasicProperties.ReplyTo;
                props.Headers = new Dictionary<string, object>
                {
                    {"retry-count", retryCount},
                    {"original-queue", _settings.PropostaStatusRequestQueue},
                    {"last-error", lastException.Message},
                    {"retry-timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds()}
                };

                _channel.BasicPublish(
                    exchange: _settings.ExchangeName,
                    routingKey: "proposta.status.request",
                    basicProperties: props,
                    body: e.Body);

                _logger.LogInformation($"Message requeued successfully with retry count {retryCount}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to requeue message with retry count {retryCount}");
                throw;
            }
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }
    }
}
