using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.ContratosPropostas
{
    public class DetailsModel : PageModel
    {
        private readonly IContratoPropostaService _contratoPropostaService;

        public DetailsModel(IContratoPropostaService contratoPropostaService)
        {
            _contratoPropostaService = contratoPropostaService;
        }

        public ContratoPropostaDto? ContratoProposta { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            ContratoProposta = await _contratoPropostaService.GetContratoPropostaByIdAsync(id);

            if (ContratoProposta == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "ID inválido para exclusão.";
                return RedirectToPage("./Index");
            }

            var success = await _contratoPropostaService.DeleteContratoPropostaAsync(id);
            
            if (success)
            {
                TempData["SuccessMessage"] = "Contrato de proposta excluído com sucesso.";
            }
            else
            {
                TempData["ErrorMessage"] = "Erro ao excluir o contrato de proposta.";
            }

            return RedirectToPage("./Index");
        }
    }
}
