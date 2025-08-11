using System.ComponentModel.DataAnnotations;

namespace InsurancePropostaService.DTOs
{
    public class GerarContratoPropostaDto
    {
        [Required(ErrorMessage = "ID da proposta é obrigatório")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        public string UserId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Data de início da vigência é obrigatória")]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required(ErrorMessage = "Data de fim da vigência é obrigatória")]
        public DateOnly DataVigenciaFim { get; set; }
    }

    public class ContratoGeradoDto
    {
        public string Id { get; set; } = string.Empty;
        public PropostaDto Proposta { get; set; } = new PropostaDto();
        public DateOnly DataVigenciaInicio { get; set; }
        public DateOnly DataVigenciaFim { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
