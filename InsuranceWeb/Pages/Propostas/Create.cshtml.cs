using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Propostas
{
    public class CreateModel : PageModel
    {
        private readonly IPropostaService _propostaService;

        public CreateModel(IPropostaService propostaService)
        {
            _propostaService = propostaService;
        }

        [BindProperty]
        public CreatePropostaDto Proposta { get; set; } = new();
        
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
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
                var result = await _propostaService.CreatePropostaAsync(Proposta);
                if (result != null)
                {
                    TempData["SuccessMessage"] = "Proposta criada com sucesso!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Erro ao criar a proposta. Verifique se o serviço está disponível.";
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
