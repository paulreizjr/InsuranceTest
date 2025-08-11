using InsuranceWeb.DTOs;

namespace InsuranceWeb.Services
{
    public interface IContratoPropostaService
    {
        Task<IEnumerable<ContratoPropostaDto>?> GetAllContratoPropostasAsync();
        Task<ContratoPropostaDto?> GetContratoPropostaByIdAsync(string id);
        Task<bool> DeleteContratoPropostaAsync(string id);
    }
}
