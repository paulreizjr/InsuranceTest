using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Propostas
{
    public class EditModel : PageModel
    {
        private readonly IPropostaService _propostaService;

        public EditModel(IPropostaService propostaService)
        {
            _propostaService = propostaService;
        }

        [BindProperty]
        public UpdatePropostaDto Proposta { get; set; } = new();
        
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
                var proposta = await _propostaService.GetPropostaByIdAsync(id);
                if (proposta == null)
                {
                    return NotFound();
                }

                Proposta = new UpdatePropostaDto
                {
                    Id = proposta.Id ?? string.Empty,
                    Automovel = proposta.Automovel,
                    Condutor = proposta.Condutor,
                    ValorAutomovel = proposta.ValorAutomovel,
                    FatorPeso = proposta.FatorPeso,
                };
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
                return Page();
            }

            try
            {
                var result = await _propostaService.UpdatePropostaAsync(Proposta.Id, Proposta);
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Proposta atualizada com sucesso!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Erro ao atualizar a proposta. Verifique se o serviço está disponível.";
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
