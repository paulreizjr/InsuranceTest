using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Operacoes
{
    public class IndexModel : PageModel
    {
        private readonly IOperacoesPropostaService _operacoesService;

        public IndexModel(IOperacoesPropostaService operacoesService)
        {
            _operacoesService = operacoesService;
        }

        public IEnumerable<PropostaDto> PropostasPendentes { get; set; } = new List<PropostaDto>();
        public EstatisticasPropostaDto Estatisticas { get; set; } = new EstatisticasPropostaDto();
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Load statistics
                var estatisticas = await _operacoesService.GetEstatisticasAsync();
                if (estatisticas != null)
                {
                    Estatisticas = estatisticas;
                }

                // Load pending proposals
                var propostas = await _operacoesService.GetPropostasPendentesAsync();
                if (propostas != null)
                {
                    PropostasPendentes = propostas;
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Erro ao carregar as propostas pendentes. Verifique se o serviço está disponível.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Erro interno: {ex.Message}";
            }

            return Page();
        }
    }
}
