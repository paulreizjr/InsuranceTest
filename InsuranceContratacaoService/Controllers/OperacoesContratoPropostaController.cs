using InsuranceContratacaoService.DTOs;
using InsuranceCoreBusiness.Application.Ports.Outbound;
using InsuranceCoreBusiness.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceContratacaoService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperacoesContratoPropostaController : ControllerBase
    {
        private readonly IContratoPropostaMessagePublisher _messagePublisher;
        private readonly ILogger<OperacoesContratoPropostaController> _logger;

        public OperacoesContratoPropostaController(
            IContratoPropostaMessagePublisher messagePublisher,
            ILogger<OperacoesContratoPropostaController> logger)
        {
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        [HttpGet("v1/proposta/{propostaId}/status")]
        public async Task<ActionResult<string>> GetPropostaStatus(string propostaId)
        {
            try
            {
                _logger.LogInformation($"Requesting status for proposta {propostaId}");
                
                var status = await _messagePublisher.getStatusPropostaFromService(propostaId);
                
                _logger.LogInformation($"Received status {status} for proposta {propostaId}");
                
                return Ok(new { PropostaId = propostaId, Status = status.ToString() });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Proposta {propostaId} not found: {ex.Message}");
                return NotFound(new { Message = ex.Message });
            }
            catch (TimeoutException ex)
            {
                _logger.LogError($"Timeout getting status for proposta {propostaId}: {ex.Message}");
                return StatusCode(408, new { Message = "Request timeout" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting status for proposta {propostaId}");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpPost("v1/proposta/{propostaId}/gerar-contrato")]
        public async Task<ActionResult<object>> GerarContrato(
            string propostaId,
            [FromBody] GerarContratoRequestDto request)
        {
            try
            {
                _logger.LogInformation($"Generating contract for proposta {propostaId}");

                var contratoId = await _messagePublisher.geraContratoFromService(
                    propostaId,
                    request.UserId,
                    request.DataVigenciaInicio,
                    request.DataVigenciaFim);

                _logger.LogInformation($"Contract generated successfully with ID {contratoId} for proposta {propostaId}");

                return Ok(new 
                { 
                    PropostaId = propostaId, 
                    ContratoId = contratoId,
                    Message = "Contract generated successfully" 
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Contract generation failed for proposta {propostaId}: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
            catch (TimeoutException ex)
            {
                _logger.LogError($"Timeout generating contract for proposta {propostaId}: {ex.Message}");
                return StatusCode(408, new { Message = "Request timeout" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating contract for proposta {propostaId}");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }
}
