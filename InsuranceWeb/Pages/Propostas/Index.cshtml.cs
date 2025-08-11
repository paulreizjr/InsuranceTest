using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Propostas
{
    public class IndexModel : PageModel
    {
        private readonly IPropostaService _propostaService;

        public IndexModel(IPropostaService propostaService)
        {
            _propostaService = propostaService;
        }

        public IEnumerable<PropostaDto> Propostas { get; set; } = new List<PropostaDto>();
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var propostas = await _propostaService.GetAllPropostasAsync();
                if (propostas != null)
                {
                    Propostas = propostas;
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Erro ao carregar as propostas. Verifique se o serviço está disponível.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Erro interno: {ex.Message}";
            }

            return Page();
        }

        public string GetStatusBadgeClass(EStatusProposta status)
        {
            return status switch
            {
                EStatusProposta.EmAnalise => "bg-warning text-dark",
                EStatusProposta.Aprovada => "bg-success",
                EStatusProposta.Rejeitada => "bg-danger",
                EStatusProposta.Contratada => "bg-primary",
                _ => "bg-secondary"
            };
        }

        public string GetStatusDisplayName(EStatusProposta status)
        {
            return status switch
            {
                EStatusProposta.EmAnalise => "Em Análise",
                EStatusProposta.Aprovada => "Aprovada",
                EStatusProposta.Rejeitada => "Rejeitada",
                EStatusProposta.Contratada => "Contratada",
                _ => "Desconhecido"
            };
        }
    }
}
