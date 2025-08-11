using System.ComponentModel.DataAnnotations;

namespace InsuranceWeb.DTOs
{
    public class ContratoPropostaDto
    {
        public string? Id { get; set; }
        
        [Required]
        [Display(Name = "Proposta")]
        public PropostaDto Proposta { get; set; } = new PropostaDto();
        
        [Required]
        [Display(Name = "Data de Início da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required]
        [Display(Name = "Data de Fim da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaFim { get; set; }
        
        [Display(Name = "Data de Atualização")]
        [DataType(DataType.DateTime)]
        public DateTime DataAtualizacao { get; set; }
    }

    public class CreateContratoPropostaDto
    {
        [Required]
        [Display(Name = "Proposta")]
        public PropostaDto Proposta { get; set; } = new PropostaDto();
        
        [Required]
        [Display(Name = "Data de Início da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required]
        [Display(Name = "Data de Fim da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaFim { get; set; }
    }

    public class UpdateContratoPropostaDto
    {
        [Required]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Proposta")]
        public PropostaDto Proposta { get; set; } = new PropostaDto();
        
        [Required]
        [Display(Name = "Data de Início da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required]
        [Display(Name = "Data de Fim da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaFim { get; set; }
    }
}
