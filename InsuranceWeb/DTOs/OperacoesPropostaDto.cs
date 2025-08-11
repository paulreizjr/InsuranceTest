using System.ComponentModel.DataAnnotations;

namespace InsuranceWeb.DTOs
{
    public class AprovarPropostaDto
    {
        [Required(ErrorMessage = "ID da proposta é obrigatório")]
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Display(Name = "Comentário")]
        public string? Comentario { get; set; }
    }

    public class ReprovarPropostaDto
    {
        [Required(ErrorMessage = "ID da proposta é obrigatório")]
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Motivo da reprovação é obrigatório")]
        [Display(Name = "Motivo da Reprovação")]
        public string MotivoReprovacao { get; set; } = string.Empty;
    }

    public class OperacaoPropostaResultDto
    {
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Display(Name = "Status Anterior")]
        public string StatusAnterior { get; set; } = string.Empty;
        
        [Display(Name = "Status Atual")]
        public string StatusAtual { get; set; } = string.Empty;
        
        [Display(Name = "Data da Operação")]
        [DataType(DataType.DateTime)]
        public DateTime DataOperacao { get; set; }
        
        [Display(Name = "Mensagem")]
        public string Mensagem { get; set; } = string.Empty;
        
        [Display(Name = "Sucesso")]
        public bool Sucesso { get; set; }
    }

    public class EstatisticasPropostaDto
    {
        [Display(Name = "Total")]
        public int Total { get; set; }
        
        [Display(Name = "Em Análise")]
        public int EmAnalise { get; set; }
        
        [Display(Name = "Aprovadas")]
        public int Aprovadas { get; set; }
        
        [Display(Name = "Rejeitadas")]
        public int Rejeitadas { get; set; }

        [Display(Name = "Contratadas")]
        public int Contratadas { get; set; }

        [Display(Name = "Data da Consulta")]
        [DataType(DataType.DateTime)]
        public DateTime DataConsulta { get; set; }
    }

    public class ValidacaoOperacaoDto
    {
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Display(Name = "Status Atual")]
        public string StatusAtual { get; set; } = string.Empty;
        
        [Display(Name = "Pode Aprovar")]
        public bool PodeAprovar { get; set; }
        
        [Display(Name = "Pode Reprovar")]
        public bool PodeReprovar { get; set; }
        
        [Display(Name = "Mensagem")]
        public string Mensagem { get; set; } = string.Empty;
        
        [Display(Name = "Data da Consulta")]
        [DataType(DataType.DateTime)]
        public DateTime DataConsulta { get; set; }
    }

    public class PropostaStatusDto
    {
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;
    }

    public class GerarContratoRequestDto
    {
        [Required(ErrorMessage = "ID da proposta é obrigatório")]
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "ID do usuário é obrigatório")]
        [Display(Name = "ID do Usuário")]
        public string UserId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Data de início da vigência é obrigatória")]
        [Display(Name = "Data de Início da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaInicio { get; set; }
        
        [Required(ErrorMessage = "Data de fim da vigência é obrigatória")]
        [Display(Name = "Data de Fim da Vigência")]
        [DataType(DataType.Date)]
        public DateOnly DataVigenciaFim { get; set; }
    }

    public class GerarContratoResultDto
    {
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;
        
        [Display(Name = "ID do Contrato")]
        public string ContratoId { get; set; } = string.Empty;
        
        [Display(Name = "Mensagem")]
        public string Message { get; set; } = string.Empty;
    }
}
