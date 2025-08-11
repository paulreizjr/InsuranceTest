using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.ContratosPropostas
{
    public class IndexModel : PageModel
    {
        private readonly IContratoPropostaService _contratoPropostaService;

        public IndexModel(IContratoPropostaService contratoPropostaService)
        {
            _contratoPropostaService = contratoPropostaService;
        }

        public IEnumerable<ContratoPropostaDto>? ContratosPropostas { get; set; }

        public async Task OnGetAsync()
        {
            ContratosPropostas = await _contratoPropostaService.GetAllContratoPropostasAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["ErrorMessage"] = "ID inválido para exclusão.";
                return RedirectToPage();
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

            return RedirectToPage();
        }
    }
}
