using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Propostas
{
    public class DeleteModel : PageModel
    {
        private readonly IPropostaService _propostaService;

        public DeleteModel(IPropostaService propostaService)
        {
            _propostaService = propostaService;
        }

        [BindProperty]
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
                Proposta = await _propostaService.GetPropostaByIdAsync(id);
                if (Proposta == null)
                {
                    return NotFound();
                }
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
            if (Proposta?.Id == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _propostaService.DeletePropostaAsync(Proposta.Id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Proposta excluída com sucesso!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Erro ao excluir a proposta. Verifique se o serviço está disponível.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Erro interno: {ex.Message}";
            }

            // Reload proposta data for display if deletion failed
            try
            {
                Proposta = await _propostaService.GetPropostaByIdAsync(Proposta.Id);
            }
            catch
            {
                // If we can't reload, redirect to index
                return RedirectToPage("./Index");
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
