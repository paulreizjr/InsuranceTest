using InsuranceWeb.DTOs;

namespace InsuranceWeb.Services
{
    public interface IPropostaService
    {
        Task<IEnumerable<PropostaDto>?> GetAllPropostasAsync();
        Task<PropostaDto?> GetPropostaByIdAsync(string id);
        Task<IEnumerable<PropostaDto>?> GetPropostasByStatusAsync(string status);
        Task<PropostaDto?> CreatePropostaAsync(CreatePropostaDto createDto);
        Task<PropostaDto?> UpdatePropostaAsync(string id, UpdatePropostaDto updateDto);
        Task<bool> DeletePropostaAsync(string id);
    }
}
