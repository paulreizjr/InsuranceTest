using InsuranceWeb.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace InsuranceWeb.Services
{
    public class PropostaService : IPropostaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public PropostaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration.GetConnectionString("PropostaApi") ?? "https://localhost:7001/api/proposta/v1";
        }

        public async Task<IEnumerable<PropostaDto>?> GetAllPropostasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
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

        public async Task<PropostaDto?> GetPropostaByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PropostaDto>(json);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<PropostaDto>?> GetPropostasByStatusAsync(string status)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/status/{status}");
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

        public async Task<PropostaDto?> CreatePropostaAsync(CreatePropostaDto createDto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(createDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync(_baseUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PropostaDto>(responseJson);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<PropostaDto?> UpdatePropostaAsync(string id, UpdatePropostaDto updateDto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync($"{_baseUrl}/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<PropostaDto>(responseJson);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeletePropostaAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
