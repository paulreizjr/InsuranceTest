using InsuranceWeb.DTOs;
using InsuranceWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InsuranceWeb.Pages.Operacoes
{
    public class GerarContratoModel : PageModel
    {
        private readonly IOperacoesContratoService _operacoesContratoService;
        private readonly IPropostaService _propostaService;

        public GerarContratoModel(IOperacoesContratoService operacoesContratoService, IPropostaService propostaService)
        {
            _operacoesContratoService = operacoesContratoService;
            _propostaService = propostaService;
        }

        [BindProperty]
        public GerarContratoRequestDto GerarContratoRequest { get; set; } = new();
        
        public PropostaDto? Proposta { get; set; }
        public GerarContratoResultDto? ContratoResult { get; set; }
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

                // Check if proposal is approved
                if (Proposta.StatusProposta != EStatusProposta.Aprovada)
                {
                    HasError = true;
                    ErrorMessage = "Só é possível gerar contrato para propostas aprovadas.";
                    return Page();
                }

                GerarContratoRequest.PropostaId = id;
                
                // Set default dates
                GerarContratoRequest.DataVigenciaInicio = DateOnly.FromDateTime(DateTime.Today);
                GerarContratoRequest.DataVigenciaFim = DateOnly.FromDateTime(DateTime.Today.AddYears(1));
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
                // Validate date range
                if (GerarContratoRequest.DataVigenciaFim <= GerarContratoRequest.DataVigenciaInicio)
                {
                    ModelState.AddModelError("GerarContratoRequest.DataVigenciaFim", "A data de fim deve ser posterior à data de início.");
                    await ReloadPropostaAsync();
                    return Page();
                }

                ContratoResult = await _operacoesContratoService.GerarContratoAsync(GerarContratoRequest);
                if (ContratoResult != null)
                {
                    TempData["SuccessMessage"] = $"Contrato gerado com sucesso! ID: {ContratoResult.ContratoId}";
                    // Reload proposta to show updated info
                    await ReloadPropostaAsync();
                }
                else
                {
                    HasError = true;
                    ErrorMessage = "Erro ao gerar o contrato. Verifique se o serviço está disponível.";
                    await ReloadPropostaAsync();
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Erro interno: {ex.Message}";
                await ReloadPropostaAsync();
            }

            return Page();
        }

        private async Task ReloadPropostaAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(GerarContratoRequest.PropostaId))
                {
                    Proposta = await _propostaService.GetPropostaByIdAsync(GerarContratoRequest.PropostaId);
                }
            }
            catch
            {
                // Ignore errors when reloading data
            }
        }

        public string GetStatusBadgeClass(EStatusProposta status)
        {
            return status switch
            {
                EStatusProposta.EmAnalise => "bg-warning text-dark",
                EStatusProposta.Aprovada => "bg-success",
                EStatusProposta.Rejeitada => "bg-danger",
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
                _ => "Desconhecido"
            };
        }
    }
}
