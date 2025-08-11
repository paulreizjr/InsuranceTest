using InsuranceCoreBusiness.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace InsurancePropostaService.DTOs
{
    public class CreatePropostaDto
    {
        [Required(ErrorMessage = "Automóvel é obrigatório")]
        public string Automovel { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Valor do automóvel é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do automóvel deve ser maior que zero")]
        public decimal ValorAutomovel { get; set; }
        
        [Required(ErrorMessage = "Fator peso é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fator peso deve ser maior que zero")]
        public decimal FatorPeso { get; set; }
        
        [Required(ErrorMessage = "Condutor é obrigatório")]
        public string Condutor { get; set; } = string.Empty;
        
        public EStatusProposta StatusProposta { get; set; }
        
        public decimal ValorProposta { get; set; }
    }

    public class UpdatePropostaDto
    {
        [Required(ErrorMessage = "ID é obrigatório")]
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Automóvel é obrigatório")]
        public string Automovel { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Valor do automóvel é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do automóvel deve ser maior que zero")]
        public decimal ValorAutomovel { get; set; }
        
        [Required(ErrorMessage = "Fator peso é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fator peso deve ser maior que zero")]
        public decimal FatorPeso { get; set; }
        
        [Required(ErrorMessage = "Condutor é obrigatório")]
        public string Condutor { get; set; } = string.Empty;
        
        public EStatusProposta StatusProposta { get; set; }
        
        public decimal ValorProposta { get; set; }
    }
}
