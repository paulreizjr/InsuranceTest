using InsuranceWeb.DTOs;

namespace InsuranceWeb.Services
{
    public interface IOperacoesPropostaService
    {
        Task<OperacaoPropostaResultDto?> AprovarPropostaAsync(AprovarPropostaDto aprovarDto);
        Task<OperacaoPropostaResultDto?> ReprovarPropostaAsync(ReprovarPropostaDto reprovarDto);
        Task<ValidacaoOperacaoDto?> ValidarOperacaoAsync(string propostaId);
        Task<IEnumerable<PropostaDto>?> GetPropostasPendentesAsync();
        Task<EstatisticasPropostaDto?> GetEstatisticasAsync();
    }
}
