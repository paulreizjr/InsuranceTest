using InsuranceWeb.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace InsuranceWeb.Services
{
    public class OperacoesPropostaService : IOperacoesPropostaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public OperacoesPropostaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration.GetConnectionString("OperacoesPropostaApi") ?? "https://localhost:7001/api/operacoesproposta/v1";
        }

        public async Task<OperacaoPropostaResultDto?> AprovarPropostaAsync(AprovarPropostaDto aprovarDto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(aprovarDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/aprovar", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<OperacaoPropostaResultDto>(responseJson);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<OperacaoPropostaResultDto?> ReprovarPropostaAsync(ReprovarPropostaDto reprovarDto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(reprovarDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/reprovar", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<OperacaoPropostaResultDto>(responseJson);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ValidacaoOperacaoDto?> ValidarOperacaoAsync(string propostaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/validar-operacao/{propostaId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ValidacaoOperacaoDto>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<PropostaDto>?> GetPropostasPendentesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/pendentes");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<PropostaDto>>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<EstatisticasPropostaDto?> GetEstatisticasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/estatisticas");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EstatisticasPropostaDto>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
