using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceContratacaoService.DTOs
{
    public class RabbitMQSettings
    {
        public string ConnectionString { get; set; } = "amqp://localhost:5672";
        public string PropostaStatusRequestQueue { get; set; } = "proposta-status-request";
        public string PropostaStatusResponseQueue { get; set; } = "proposta-status-response";
        public string GeraContratoRequestQueue { get; set; } = "gera-contrato-request";
        public string GeraContratoResponseQueue { get; set; } = "gera-contrato-response";
        public string ExchangeName { get; set; } = "insurance-exchange";
        public string DeadLetterQueue { get; set; } = "insurance-dead-letter";
        public int MaxRetries { get; set; } = 3;
        public int MessageTtlMs { get; set; } = 300000; // 5 minutes
        public int RequestTimeoutSeconds { get; set; } = 30;
        public int CircuitBreakerThreshold { get; set; } = 5;
        public int CircuitBreakerTimeoutMinutes { get; set; } = 5;
    }
}
