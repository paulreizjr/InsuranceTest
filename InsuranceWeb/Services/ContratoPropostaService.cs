using InsuranceWeb.DTOs;
using System.Text.Json;

namespace InsuranceWeb.Services
{
    public class ContratoPropostaService : IContratoPropostaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public ContratoPropostaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _baseUrl = _configuration.GetConnectionString("ContratoPropostaApi") ?? throw new InvalidOperationException("ContratoPropostaApi connection string not found");
        }

        public async Task<IEnumerable<ContratoPropostaDto>?> GetAllContratoPropostasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<IEnumerable<ContratoPropostaDto>>(json, options);
                }
                
                return new List<ContratoPropostaDto>();
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error getting contract proposals: {ex.Message}");
                return new List<ContratoPropostaDto>();
            }
        }

        public async Task<ContratoPropostaDto?> GetContratoPropostaByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<ContratoPropostaDto>(json, options);
                }
                
                return null;
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error getting contract proposal {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeleteContratoPropostaAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error deleting contract proposal {id}: {ex.Message}");
                return false;
            }
        }
    }
}
