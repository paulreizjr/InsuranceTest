using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace InsuranceWeb.Pages.Operacoes
{
    public class StatusModel : PageModel
    {
        private readonly IOperacoesContratoService _operacoesContratoService;

        public StatusModel(IOperacoesContratoService operacoesContratoService)
        {
            _operacoesContratoService = operacoesContratoService;
        }

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "ID da proposta é obrigatório")]
        [Display(Name = "ID da Proposta")]
        public string PropostaId { get; set; } = string.Empty;

        public PropostaStatusDto? PropostaStatus { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(PropostaId))
            {
                try
                {
                    PropostaStatus = await _operacoesContratoService.GetPropostaStatusAsync(PropostaId);
                    if (PropostaStatus == null)
                    {
                        HasError = true;
                        ErrorMessage = "Proposta não encontrada ou erro ao consultar o status.";
                    }
                }
                catch (Exception ex)
                {
                    HasError = true;
                    ErrorMessage = $"Erro ao consultar o status: {ex.Message}";
                }
            }

            return Page();
        }

        public string GetStatusBadgeClass(string status)
        {
            return status?.ToLower() switch
            {
                "emanalise" => "bg-warning text-dark",
                "aprovada" => "bg-success",
                "rejeitada" => "bg-danger",
                "contratada" => "bg-info text-dark",
                _ => "bg-secondary"
            };
        }

        public string GetStatusDisplayName(string status)
        {
            return status?.ToLower() switch
            {
                "emanalise" => "Em Análise",
                "aprovada" => "Aprovada",
                "rejeitada" => "Rejeitada",
                "contratada" => "Contratada",
                _ => "Desconhecido"
            };
        }
    }
}
