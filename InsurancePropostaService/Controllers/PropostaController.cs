using InsuranceCoreBusiness.Application.Ports.Inbound;
using InsuranceCoreBusiness.Domain.Entities;
using InsuranceCoreBusiness.Domain.Enums;
using InsurancePropostaService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InsurancePropostaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropostaController : ControllerBase
    {
        private readonly ICrudPropostaUC _crudPropostaUC;

        public PropostaController(ICrudPropostaUC crudPropostaUC)
        {
            _crudPropostaUC = crudPropostaUC;
        }

        /// <summary>
        /// Retrieves all proposals
        /// </summary>
        /// <returns>List of proposals</returns>
        [HttpGet("v1/")]
        public async Task<ActionResult<IEnumerable<PropostaDto>>> GetAll()
        {
            try
            {
                var propostas = await _crudPropostaUC.GetAllPropostaAsync();
                var propostasDto = propostas.Select(MapToDto);
                return Ok(propostasDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a specific proposal by ID
        /// </summary>
        /// <param name="id">The proposal ID</param>
        /// <returns>The proposal if found</returns>
        [HttpGet("v1/{id}")]
        public async Task<ActionResult<PropostaDto>> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID cannot be null or empty");
                }

                var proposta = await _crudPropostaUC.GetPropostaByIdAsync(id);
                if (proposta == null)
                {
                    return NotFound($"Proposal with ID {id} not found");
                }

                return Ok(MapToDto(proposta));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves proposals by status
        /// </summary>
        /// <param name="status">The proposal status</param>
        /// <returns>List of proposals with the specified status</returns>
        [HttpGet("v1/status/{status}")]
        public async Task<ActionResult<IEnumerable<PropostaDto>>> GetByStatus(string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(status))
                {
                    return BadRequest("Status cannot be null or empty");
                }

                // Validate if the status is a valid enum value
                if (!Enum.TryParse<EStatusProposta>(status, true, out _))
                {
                    var validStatuses = string.Join(", ", Enum.GetNames<EStatusProposta>());
                    return BadRequest($"Invalid status. Valid values are: {validStatuses}");
                }

                var propostas = await _crudPropostaUC.GetPropostasByStatusAsync(status);
                var propostasDto = propostas.Select(MapToDto);
                return Ok(propostasDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new proposal
        /// </summary>
        /// <param name="createDto">The proposal data</param>
        /// <returns>The created proposal</returns>
        [HttpPost("v1/")]
        public async Task<ActionResult<PropostaDto>> Create([FromBody] CreatePropostaDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var proposta = MapFromCreateDto(createDto);
                proposta.id = Guid.NewGuid().ToString();
                proposta.dataAtualizacao = DateTime.UtcNow;

                var result = await _crudPropostaUC.AddPropostaAsync(proposta);
                if (result > 0)
                {
                    var createdProposta = await _crudPropostaUC.GetPropostaByIdAsync(proposta.id);
                    return CreatedAtAction(nameof(GetById), new { id = proposta.id }, MapToDto(createdProposta));
                }

                return BadRequest("Failed to create proposal");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing proposal
        /// </summary>
        /// <param name="id">The proposal ID</param>
        /// <param name="updateDto">The updated proposal data</param>
        /// <returns>The updated proposal</returns>
        [HttpPut("v1/{id}")]
        public async Task<ActionResult<PropostaDto>> Update(string id, [FromBody] UpdatePropostaDto updateDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return BadRequest("ID cannot be null or empty");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (id != updateDto.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var existingProposta = await _crudPropostaUC.GetPropostaByIdAsync(id);
                if (existingProposta == null)
                {
                    return NotFound($"Proposal with ID {id} not found");
                }

                var proposta = MapFromUpdateDto(updateDto);
                proposta.dataAtualizacao = DateTime.UtcNow;

                var result = await _crudPropostaUC.UpdatePropostaAsync(proposta);
                if (result > 0)
                {
                    var updatedProposta = await _crudPropostaUC.GetPropostaByIdAsync(id);
                    return Ok(MapToDto(updatedProposta));
                }

                return BadRequest("Failed to update proposal");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a proposal
        /// </summary>
        /// <param name="id">The proposal ID</param>
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

                var existingProposta = await _crudPropostaUC.GetPropostaByIdAsync(id);
                if (existingProposta == null)
                {
                    return NotFound($"Proposal with ID {id} not found");
                }

                var result = await _crudPropostaUC.DeletePropostaAsync(id);
                if (result > 0)
                {
                    return NoContent();
                }

                return BadRequest("Failed to delete proposal");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #region Mapping Methods

        private PropostaDto MapToDto(Proposta entity)
        {
            return new PropostaDto
            {
                Id = entity.id,
                Automovel = entity.automovel ?? string.Empty,
                ValorAutomovel = entity.valorAutomovel,
                FatorPeso = entity.fatorPeso,
                Condutor = entity.condutor ?? string.Empty,
                StatusProposta = entity.statusProposta,
                ValorProposta = entity.valorProposta,
                DataAtualizacao = entity.dataAtualizacao
            };
        }

        private Proposta MapFromCreateDto(CreatePropostaDto dto)
        {
            return new Proposta
            {
                automovel = dto.Automovel,
                valorAutomovel = dto.ValorAutomovel,
                fatorPeso = dto.FatorPeso,
                condutor = dto.Condutor,
                statusProposta = dto.StatusProposta,
                valorProposta = dto.ValorProposta
            };
        }

        private Proposta MapFromUpdateDto(UpdatePropostaDto dto)
        {
            return new Proposta
            {
                id = dto.Id,
                automovel = dto.Automovel,
                valorAutomovel = dto.ValorAutomovel,
                fatorPeso = dto.FatorPeso,
                condutor = dto.Condutor,
                statusProposta = dto.StatusProposta,
                valorProposta = dto.ValorProposta
            };
        }

        #endregion
    }
}
