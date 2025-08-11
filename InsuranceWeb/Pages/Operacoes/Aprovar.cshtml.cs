using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Operacoes
{
    public class AprovarModel : PageModel
    {
        private readonly IOperacoesPropostaService _operacoesService;
        private readonly IPropostaService _propostaService;

        public AprovarModel(IOperacoesPropostaService operacoesService, IPropostaService propostaService)
        {
            _operacoesService = operacoesService;
            _propostaService = propostaService;
        }

        [BindProperty]
        public AprovarPropostaDto AprovarDto { get; set; } = new();
        
        public PropostaDto? Proposta { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                // Load proposal details
                Proposta = await _propostaService.GetPropostaByIdAsync(id);
                if (Proposta == null)
                {
                    return NotFound();
                }

                // Validate if proposal can be approved
                var validacao = await _operacoesService.ValidarOperacaoAsync(id);
                if (validacao == null || !validacao.PodeAprovar)
                {
                    HasError = true;
                    ErrorMessage = validacao?.Mensagem ?? "Esta proposta não pode ser aprovada no momento.";
                    return Page();
                }

                AprovarDto.PropostaId = id;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Erro ao carregar a proposta: {ex.Message}";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Reload proposta data
                await ReloadPropostaAsync();
                return Page();
            }

            try
            {
                var result = await _operacoesService.AprovarPropostaAsync(AprovarDto);
                if (result != null && result.Sucesso)
                {
                    TempData["SuccessMessage"] = $"Proposta aprovada com sucesso! {result.Mensagem}";
                    return RedirectToPage("./Index");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = result?.Mensagem ?? "Erro ao aprovar a proposta. Verifique se o serviço está disponível.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Erro interno: {ex.Message}";
            }

            // Reload proposta data on error
            await ReloadPropostaAsync();
            return Page();
        }

        private async Task ReloadPropostaAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(AprovarDto.PropostaId))
                {
                    Proposta = await _propostaService.GetPropostaByIdAsync(AprovarDto.PropostaId);
                }
            }
            catch
            {
                // Ignore errors when reloading data
            }
        }

        public string GetStatusDisplayName(EStatusProposta status)
        {
            return status switch
            {
                EStatusProposta.EmAnalise => "Em Análise",
                EStatusProposta.Aprovada => "Aprovada",
                EStatusProposta.Rejeitada => "Rejeitada",
                _ => "Desconhecido"
            };
        }
    }
}
