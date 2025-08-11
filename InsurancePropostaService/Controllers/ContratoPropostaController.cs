using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Domain.Entities;
using InsurancePropostaService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePropostaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContratoPropostaController : ControllerBase
    {
        private readonly ICrudContratoPropostaUC _crudContratoPropostaUC;

        public ContratoPropostaController(ICrudContratoPropostaUC crudContratoPropostaUC)
        {
            _crudContratoPropostaUC = crudContratoPropostaUC;
        }

        /// <summary>
        /// Retrieves all contract proposals
        /// </summary>
        /// <returns>List of contract proposals</returns>
        [HttpGet("v1/")]
        public async Task<ActionResult<IEnumerable<ContratoPropostaDto>>> GetAll()
        {
            try
            {
                var contratos = await _crudContratoPropostaUC.GetAllContratoPropostaAsync();
                var contratosDto = contratos.Select(MapToDto);
                return Ok(contratosDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a specific contract proposal by ID
        /// </summary>
        /// <param name="id">The contract proposal ID</param>
        /// <returns>The contract proposal if found</returns>
        [HttpGet("v1/{id}")]
        public async Task<ActionResult<ContratoPropostaDto>> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID cannot be null or empty");
                }

                var contrato = await _crudContratoPropostaUC.GetContratoPropostaByIdAsync(id);
                if (contrato == null)
                {
                    return NotFound($"Contract proposal with ID {id} not found");
                }

                return Ok(MapToDto(contrato));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a contract proposal
        /// </summary>
        /// <param name="id">The contract proposal ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("v1/{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID cannot be null or empty");
                }

                var existingContrato = await _crudContratoPropostaUC.GetContratoPropostaByIdAsync(id);
                if (existingContrato == null)
                {
                    return NotFound($"Contract proposal with ID {id} not found");
                }

                var result = await _crudContratoPropostaUC.DeleteContratoPropostaAsync(id);
                if (result > 0)
                {
                    return NoContent();
                }

                return BadRequest("Failed to delete contract proposal");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #region Mapping Methods

        private ContratoPropostaDto MapToDto(ContratoProposta entity)
        {
            return new ContratoPropostaDto
            {
                Id = entity.id,
                Proposta = MapPropostaToDto(entity.proposta),
                DataVigenciaInicio = entity.dataVigenciaInicio,
                DataVigenciaFim = entity.dataVigenciaFim,
                DataAtualizacao = entity.dataAtualizacao
            };
        }

        private PropostaDto MapPropostaToDto(Proposta proposta)
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
