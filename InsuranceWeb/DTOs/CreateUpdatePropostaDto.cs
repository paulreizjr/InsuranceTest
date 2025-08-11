using System.ComponentModel.DataAnnotations;

namespace InsuranceWeb.DTOs
{
    public class CreatePropostaDto
    {
        [Required(ErrorMessage = "Automóvel é obrigatório")]
        [Display(Name = "Automóvel")]
        public string Automovel { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Valor do automóvel é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do automóvel deve ser maior que zero")]
        [Display(Name = "Valor do Automóvel")]
        [DataType(DataType.Currency)]
        public decimal ValorAutomovel { get; set; }
        
        [Required(ErrorMessage = "Fator peso é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fator peso deve ser maior que zero")]
        [Display(Name = "Fator Peso")]
        public decimal FatorPeso { get; set; }
        
        [Required(ErrorMessage = "Condutor é obrigatório")]
        [Display(Name = "Condutor")]
        public string Condutor { get; set; } = string.Empty;
        
        [Display(Name = "Status da Proposta")]
        public EStatusProposta StatusProposta { get; set; } = EStatusProposta.EmAnalise;

        [Display(Name = "Valor da Proposta")]
        [DataType(DataType.Currency)]
        public decimal ValorProposta { get; set; }
    }

    public class UpdatePropostaDto
    {
        [Required(ErrorMessage = "ID é obrigatório")]
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Automóvel é obrigatório")]
        [Display(Name = "Automóvel")]
        public string Automovel { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Valor do automóvel é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor do automóvel deve ser maior que zero")]
        [Display(Name = "Valor do Automóvel")]
        [DataType(DataType.Currency)]
        public decimal ValorAutomovel { get; set; }
        
        [Required(ErrorMessage = "Fator peso é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fator peso deve ser maior que zero")]
        [Display(Name = "Fator Peso")]
        public decimal FatorPeso { get; set; }
        
        [Required(ErrorMessage = "Condutor é obrigatório")]
        [Display(Name = "Condutor")]
        public string Condutor { get; set; } = string.Empty;
        
        [Display(Name = "Status da Proposta")]
        public EStatusProposta StatusProposta { get; set; }
        
        [Display(Name = "Valor da Proposta")]
        [DataType(DataType.Currency)]
        public decimal ValorProposta { get; set; }
    }
}
