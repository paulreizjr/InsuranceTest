using System.ComponentModel.DataAnnotations;

namespace InsurancePropostaService.DTOs
{
    public class ContratoPropostaDto
    {
        public string? Id { get; set; }
        
        [Required]
        public PropostaDto Proposta { get; set; } = new PropostaDto();
        
        [Required]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required]
        public DateOnly DataVigenciaFim { get; set; }
        
        public DateTime DataAtualizacao { get; set; }
    }

    public class CreateContratoPropostaDto
    {
        [Required]
        public PropostaDto Proposta { get; set; } = new PropostaDto();
        
        [Required]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required]
        public DateOnly DataVigenciaFim { get; set; }
    }

    public class UpdateContratoPropostaDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        public PropostaDto Proposta { get; set; } = new PropostaDto();
        
        [Required]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required]
        public DateOnly DataVigenciaFim { get; set; }
    }
}
