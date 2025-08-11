using InsuranceContratacaoService.DTOs;
using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Domain.Enums;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace InsuranceContratacaoService.Services
{
    public class ContratoPropostaMessagePublisher : IContratoPropostaMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMQSettings _settings;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<EStatusProposta>> _pendingStatusRequests;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _pendingContratoRequests;
        private readonly string _responseStatusQueueName;
        private readonly string _responseContratoQueueName;
        
        // Circuit breaker fields
        private int _consecutiveFailures = 0;
        private DateTime _lastFailureTime = DateTime.MinValue;
        private readonly object _circuitBreakerLock = new object();

        public ContratoPropostaMessagePublisher(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
            _pendingStatusRequests = new ConcurrentDictionary<string, TaskCompletionSource<EStatusProposta>>();
            _pendingContratoRequests = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_settings.ConnectionString)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Infrastructure setup is handled by RabbitMQInfrastructureService
            // This publisher only creates temporary response queues and consumers

            // Create temporary response queues for receiving responses
            _responseStatusQueueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_responseStatusQueueName, _settings.ExchangeName, "proposta.status.response");

            _responseContratoQueueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(_responseContratoQueueName, _settings.ExchangeName, "gera.contrato.response");

            var statusConsumer = new EventingBasicConsumer(_channel);
            statusConsumer.Received += OnResponseStatusReceived;
            _channel.BasicConsume(_responseStatusQueueName, true, statusConsumer);

            var contratoConsumer = new EventingBasicConsumer(_channel);
            contratoConsumer.Received += OnResponseContratoReceived;
            _channel.BasicConsume(_responseContratoQueueName, true, contratoConsumer);
        }

        public async Task<EStatusProposta> getStatusPropostaFromService(string propostaId)
        {
            // Check circuit breaker
            lock (_circuitBreakerLock)
            {
                if (_consecutiveFailures >= _settings.CircuitBreakerThreshold && 
                    DateTime.UtcNow - _lastFailureTime < TimeSpan.FromMinutes(_settings.CircuitBreakerTimeoutMinutes))
                {
                    throw new InvalidOperationException($"Circuit breaker is open. Service temporarily unavailable. Consecutive failures: {_consecutiveFailures}");
                }
            }

            var requestId = Guid.NewGuid().ToString();
            var request = new PropostaStatusRequestDto
            {
                PropostaId = propostaId,
                RequestId = requestId
            };

            var tcs = new TaskCompletionSource<EStatusProposta>();
            _pendingStatusRequests[requestId] = tcs;

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
            var properties = _channel.CreateBasicProperties();
            properties.ReplyTo = _responseStatusQueueName;
            properties.CorrelationId = requestId;

            try
            {
                _channel.BasicPublish(
                    exchange: _settings.ExchangeName,
                    routingKey: "proposta.status.request",
                    basicProperties: properties,
                    body: messageBody);

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.RequestTimeoutSeconds));
                cancellationTokenSource.Token.Register(() =>
                {
                    if (_pendingStatusRequests.TryRemove(requestId, out var timeoutTcs))
                    {
                        timeoutTcs.SetException(new TimeoutException("Request timed out"));
                    }
                });

                var result = await tcs.Task;
                
                // Reset circuit breaker on success
                lock (_circuitBreakerLock)
                {
                    _consecutiveFailures = 0;
                }
                
                return result;
            }
            catch (TimeoutException)
            {
                // Update circuit breaker on timeout
                lock (_circuitBreakerLock)
                {
                    _consecutiveFailures++;
                    _lastFailureTime = DateTime.UtcNow;
                }
                throw;
            }
            catch (Exception)
            {
                // Update circuit breaker on other exceptions
                lock (_circuitBreakerLock)
                {
                    _consecutiveFailures++;
                    _lastFailureTime = DateTime.UtcNow;
                }
                throw;
            }
        }

        public async Task<string> geraContratoFromService(string propostaId, string userId, DateOnly dataVigenciaInicio, DateOnly dataVigenciaFim)
        {
            // Check circuit breaker
            lock (_circuitBreakerLock)
            {
                if (_consecutiveFailures >= _settings.CircuitBreakerThreshold && 
                    DateTime.UtcNow - _lastFailureTime < TimeSpan.FromMinutes(_settings.CircuitBreakerTimeoutMinutes))
                {
                    throw new InvalidOperationException($"Circuit breaker is open. Service temporarily unavailable. Consecutive failures: {_consecutiveFailures}");
                }
            }

            var requestId = Guid.NewGuid().ToString();
            var request = new GeraContratoRequestDto
            {
                PropostaId = propostaId,
                UserId = userId,
                DataVigenciaInicio = dataVigenciaInicio,
                DataVigenciaFim = dataVigenciaFim,
                RequestId = requestId
            };

            var tcs = new TaskCompletionSource<string>();
            _pendingContratoRequests[requestId] = tcs;

            var messageBody = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));
            var properties = _channel.CreateBasicProperties();
            properties.ReplyTo = _responseContratoQueueName;
            properties.CorrelationId = requestId;

            try
            {
                _channel.BasicPublish(
                    exchange: _settings.ExchangeName,
                    routingKey: "gera.contrato.request",
                    basicProperties: properties,
                    body: messageBody);

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(_settings.RequestTimeoutSeconds));
                cancellationTokenSource.Token.Register(() =>
                {
                    if (_pendingContratoRequests.TryRemove(requestId, out var timeoutTcs))
                    {
                        timeoutTcs.SetException(new TimeoutException("Contract generation request timed out"));
                    }
                });

                var result = await tcs.Task;
                
                // Reset circuit breaker on success
                lock (_circuitBreakerLock)
                {
                    _consecutiveFailures = 0;
                }
                
                return result;
            }
            catch (TimeoutException)
            {
                // Update circuit breaker on timeout
                lock (_circuitBreakerLock)
                {
                    _consecutiveFailures++;
                    _lastFailureTime = DateTime.UtcNow;
                }
                throw;
            }
            catch (Exception)
            {
                // Update circuit breaker on other exceptions
                lock (_circuitBreakerLock)
                {
                    _consecutiveFailures++;
                    _lastFailureTime = DateTime.UtcNow;
                }
                throw;
            }
        }

        private void OnResponseStatusReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var responseJson = Encoding.UTF8.GetString(e.Body.ToArray());

                var response = JsonSerializer.Deserialize<PropostaStatusResponseDto>(responseJson);
                if (response != null && _pendingStatusRequests.TryRemove(response.RequestId, out var tcs))
                {
                    if (response.PropostaFound)
                    {
                        tcs.SetResult(response.StatusProposta);
                    }
                    else
                    {
                        tcs.SetException(new InvalidOperationException($"Proposta {response.PropostaId} not found"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing response: {ex.Message}");
            }
        }

        private void OnResponseContratoReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var responseJson = Encoding.UTF8.GetString(e.Body.ToArray());

                var response = JsonSerializer.Deserialize<GeraContratoResponseDto>(responseJson);
                if (response != null && _pendingContratoRequests.TryRemove(response.RequestId, out var tcs))
                {
                    if (response.Success)
                    {
                        tcs.SetResult(response.ContratoId!);
                    }
                    else
                    {
                        tcs.SetException(new InvalidOperationException($"Contract generation failed: {response.ErrorMessage}"));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing response: {ex.Message}");
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
