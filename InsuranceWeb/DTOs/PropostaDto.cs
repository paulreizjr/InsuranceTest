using System.ComponentModel.DataAnnotations;

namespace InsuranceWeb.DTOs
{
    public class PropostaDto
    {
        public string? Id { get; set; }
        
        [Required]
        [Display(Name = "Automóvel")]
        public string Automovel { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do automóvel deve ser maior que zero")]
        [Display(Name = "Valor do Automóvel")]
        [DataType(DataType.Currency)]
        public decimal ValorAutomovel { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fator peso deve ser maior que zero")]
        [Display(Name = "Fator Peso")]
        public decimal FatorPeso { get; set; }
        
        [Required]
        [Display(Name = "Condutor")]
        public string Condutor { get; set; } = string.Empty;
        
        [Display(Name = "Status da Proposta")]
        public EStatusProposta StatusProposta { get; set; }
        
        [Display(Name = "Valor da Proposta")]
        [DataType(DataType.Currency)]
        public decimal ValorProposta { get; set; }
        
        [Display(Name = "Data de Atualização")]
        [DataType(DataType.DateTime)]
        public DateTime DataAtualizacao { get; set; }
    }
}
