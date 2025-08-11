using InsuranceWeb.DTOs;

namespace InsuranceWeb.Services
{
    public interface IOperacoesContratoService
    {
        Task<PropostaStatusDto?> GetPropostaStatusAsync(string propostaId);
        Task<GerarContratoResultDto?> GerarContratoAsync(GerarContratoRequestDto request);
    }
}
