using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Domain.Enums;
using InsuranceCoreBusiness.Domain.Exceptions;
using InsurancePropostaService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePropostaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperacoesPropostaController : ControllerBase
    {
        private readonly IOperacoesPropostaUC _operacoesPropostaUC;
        private readonly ICrudPropostaUC _crudPropostaUC;

        public OperacoesPropostaController(
            IOperacoesPropostaUC operacoesPropostaUC,
            ICrudPropostaUC crudPropostaUC)
        {
            _operacoesPropostaUC = operacoesPropostaUC;
            _crudPropostaUC = crudPropostaUC;
        }

        /// <summary>
        /// Approves a proposal that is under analysis
        /// </summary>
        /// <param name="aprovarDto">The approval data</param>
        /// <returns>Result of the approval operation</returns>
        [HttpPost("v1/aprovar")]
        public async Task<ActionResult<OperacaoPropostaResultDto>> AprovarProposta([FromBody] AprovarPropostaDto aprovarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get current proposal to capture status before operation
                var propostaAntes = await _crudPropostaUC.GetPropostaByIdAsync(aprovarDto.PropostaId);
                if (propostaAntes == null)
                {
                    return NotFound($"Proposta com ID {aprovarDto.PropostaId} não encontrada");
                }

                var statusAnterior = propostaAntes.statusProposta;

                // Perform approval operation
                var result = await _operacoesPropostaUC.AprovaPropostaPorId(aprovarDto.PropostaId);

                if (result > 0)
                {
                    // Get updated proposal to confirm status change
                    var propostaDepois = await _crudPropostaUC.GetPropostaByIdAsync(aprovarDto.PropostaId);
                    
                    var response = new OperacaoPropostaResultDto
                    {
                        PropostaId = aprovarDto.PropostaId,
                        StatusAnterior = statusAnterior.ToString(),
                        StatusAtual = propostaDepois?.statusProposta.ToString() ?? "Desconhecido",
                        DataOperacao = DateTime.UtcNow,
                        Mensagem = "Proposta aprovada com sucesso",
                        Sucesso = true
                    };

                    return Ok(response);
                }

                return BadRequest("Falha ao aprovar a proposta");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (StatusPropostaInvalidoException ex)
            {
                return BadRequest($"Status da proposta inválido: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Rejects a proposal that is under analysis
        /// </summary>
        /// <param name="reprovarDto">The rejection data</param>
        /// <returns>Result of the rejection operation</returns>
        [HttpPost("v1/reprovar")]
        public async Task<ActionResult<OperacaoPropostaResultDto>> ReprovarProposta([FromBody] ReprovarPropostaDto reprovarDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get current proposal to capture status before operation
                var propostaAntes = await _crudPropostaUC.GetPropostaByIdAsync(reprovarDto.PropostaId);
                if (propostaAntes == null)
                {
                    return NotFound($"Proposta com ID {reprovarDto.PropostaId} não encontrada");
                }

                var statusAnterior = propostaAntes.statusProposta;

                // Perform rejection operation
                var result = await _operacoesPropostaUC.ReprovaPropostaPorId(reprovarDto.PropostaId);

                if (result > 0)
                {
                    // Get updated proposal to confirm status change
                    var propostaDepois = await _crudPropostaUC.GetPropostaByIdAsync(reprovarDto.PropostaId);
                    
                    var response = new OperacaoPropostaResultDto
                    {
                        PropostaId = reprovarDto.PropostaId,
                        StatusAnterior = statusAnterior.ToString(),
                        StatusAtual = propostaDepois?.statusProposta.ToString() ?? "Desconhecido",
                        DataOperacao = DateTime.UtcNow,
                        Mensagem = $"Proposta rejeitada com sucesso. Motivo: {reprovarDto.MotivoReprovacao}",
                        Sucesso = true
                    };

                    return Ok(response);
                }

                return BadRequest("Falha ao reprovar a proposta");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (StatusPropostaInvalidoException ex)
            {
                return BadRequest($"Status da proposta inválido: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks if a proposal can be approved or rejected
        /// </summary>
        /// <param name="propostaId">The proposal ID</param>
        /// <returns>Validation result</returns>
        [HttpGet("v1/validar-operacao/{propostaId}")]
        public async Task<ActionResult<object>> ValidarOperacao(string propostaId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(propostaId))
                {
                    return BadRequest("ID da proposta não pode ser nulo ou vazio");
                }

                var proposta = await _crudPropostaUC.GetPropostaByIdAsync(propostaId);
                if (proposta == null)
                {
                    return NotFound($"Proposta com ID {propostaId} não encontrada");
                }

                var podeOperar = proposta.statusProposta == EStatusProposta.EmAnalise;

                return Ok(new
                {
                    PropostaId = propostaId,
                    StatusAtual = proposta.statusProposta.ToString(),
                    PodeAprovar = podeOperar,
                    PodeReprovar = podeOperar,
                    Mensagem = podeOperar 
                        ? "Proposta pode ser aprovada ou reprovada" 
                        : $"Proposta não pode ser operada. Status atual: {proposta.statusProposta}",
                    DataConsulta = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all proposals that are under analysis (can be approved/rejected)
        /// </summary>
        /// <returns>List of proposals under analysis</returns>
        [HttpGet("v1/pendentes")]
        public async Task<ActionResult<IEnumerable<PropostaDto>>> GetPropostasPendentes()
        {
            try
            {
                var propostas = await _crudPropostaUC.GetPropostasByStatusAsync(EStatusProposta.EmAnalise.ToString());
                var propostasDto = propostas.Select(MapPropostaToDto);
                
                return Ok(propostasDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets statistics about proposal operations
        /// </summary>
        /// <returns>Statistics about proposals by status</returns>
        [HttpGet("v1/estatisticas")]
        public async Task<ActionResult<object>> GetEstatisticas()
        {
            try
            {
                var todasPropostas = await _crudPropostaUC.GetAllPropostaAsync();
                var propostas = todasPropostas.ToList();

                var estatisticas = new
                {
                    Total = propostas.Count,
                    EmAnalise = propostas.Count(p => p.statusProposta == EStatusProposta.EmAnalise),
                    Aprovadas = propostas.Count(p => p.statusProposta == EStatusProposta.Aprovada),
                    Rejeitadas = propostas.Count(p => p.statusProposta == EStatusProposta.Rejeitada),
                    Contratadas = propostas.Count(p => p.statusProposta == EStatusProposta.Contratada),
                    DataConsulta = DateTime.UtcNow
                };

                return Ok(estatisticas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        #region Helper Methods

        private PropostaDto MapPropostaToDto(InsuranceCoreBusiness.Domain.Entities.Proposta proposta)
        {
            return new PropostaDto
            {
                Id = proposta.id,
                Automovel = proposta.automovel ?? string.Empty,
                ValorAutomovel = proposta.valorAutomovel,
                FatorPeso = proposta.fatorPeso,
                Condutor = proposta.condutor ?? string.Empty,
                StatusProposta = proposta.statusProposta,
                ValorProposta = proposta.valorProposta,
                DataAtualizacao = proposta.dataAtualizacao
            };
        }

        #endregion
    }
}
