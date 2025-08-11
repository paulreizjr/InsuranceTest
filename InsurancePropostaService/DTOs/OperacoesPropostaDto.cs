using System.ComponentModel.DataAnnotations;

namespace InsurancePropostaService.DTOs
{
    public class AprovarPropostaDto
    {
        [Required(ErrorMessage = "ID da proposta é obrigatório")]
        public string PropostaId { get; set; } = string.Empty;
        
        public string? Comentario { get; set; }
    }

    public class ReprovarPropostaDto
    {
        [Required(ErrorMessage = "ID da proposta é obrigatório")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Motivo da reprovação é obrigatório")]
        public string MotivoReprovacao { get; set; } = string.Empty;
    }

    public class OperacaoPropostaResultDto
    {
        public string PropostaId { get; set; } = string.Empty;
        public string StatusAnterior { get; set; } = string.Empty;
        public string StatusAtual { get; set; } = string.Empty;
        public DateTime DataOperacao { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public bool Sucesso { get; set; }
    }
}
