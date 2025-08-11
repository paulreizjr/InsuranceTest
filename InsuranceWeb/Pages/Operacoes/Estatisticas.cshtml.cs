using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Operacoes
{
    public class EstatisticasModel : PageModel
    {
        private readonly IOperacoesPropostaService _operacoesService;

        public EstatisticasModel(IOperacoesPropostaService operacoesService)
        {
            _operacoesService = operacoesService;
        }

        public EstatisticasPropostaDto? Estatisticas { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Estatisticas = await _operacoesService.GetEstatisticasAsync();
                if (Estatisticas == null)
                {
                    HasError = true;
                    ErrorMessage = "Erro ao carregar as estatísticas. Verifique se o serviço está disponível.";
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
