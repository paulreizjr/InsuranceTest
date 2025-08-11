using InsuranceCoreBusiness.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsurancePropostaService.DTOs
{
    public class PropostaDto
    {
        public string? Id { get; set; }
        
        [Required]
        public string Automovel { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do automóvel deve ser maior que zero")]
        public decimal ValorAutomovel { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fator peso deve ser maior que zero")]
        public decimal FatorPeso { get; set; }
        
        [Required]
        public string Condutor { get; set; } = string.Empty;
        
        public EStatusProposta StatusProposta { get; set; }
        
        public decimal ValorProposta { get; set; }
        
        public DateTime DataAtualizacao { get; set; }
    }
}
