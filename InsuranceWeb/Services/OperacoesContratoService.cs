using InsuranceWeb.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace InsuranceWeb.Services
{
    public class OperacoesContratoService : IOperacoesContratoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public OperacoesContratoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration.GetConnectionString("OperacoesContratoApi") ?? "https://localhost:7002/api/operacoescontratoproposta/v1";
        }

        public async Task<PropostaStatusDto?> GetPropostaStatusAsync(string propostaId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/proposta/{propostaId}/status");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PropostaStatusDto>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<GerarContratoResultDto?> GerarContratoAsync(GerarContratoRequestDto request)
        {
            try
            {
                var requestBody = new
                {
                    UserId = request.UserId,
                    DataVigenciaInicio = request.DataVigenciaInicio,
                    DataVigenciaFim = request.DataVigenciaFim
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_baseUrl}/proposta/{request.PropostaId}/gerar-contrato", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<GerarContratoResultDto>(responseJson);
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
