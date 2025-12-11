using System.Net.Http.Json;
using LifeForge.Web.Models;

namespace LifeForge.Web.Services
{
    public class BuffInstanceService
    {
        private readonly HttpClient _httpClient;

        public BuffInstanceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BuffInstanceDto>> GetAllBuffInstancesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BuffInstanceDto>>("api/buffinstances") ?? new List<BuffInstanceDto>();
        }

        public async Task<List<BuffInstanceDto>> GetActiveBuffInstancesByCharacterIdAsync(string characterId)
        {
            return await _httpClient.GetFromJsonAsync<List<BuffInstanceDto>>($"api/buffinstances/character/{characterId}") ?? new List<BuffInstanceDto>();
        }

        public async Task<BuffInstanceDto?> GetBuffInstanceAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<BuffInstanceDto>($"api/buffinstances/{id}");
        }

        public async Task<BuffInstanceApplicationResultDto?> ActivateBuffAsync(string characterId, string buffId)
        {
            var activateDto = new { CharacterId = characterId, BuffId = buffId };
            var response = await _httpClient.PostAsJsonAsync("api/buffinstances/activate", activateDto);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BuffInstanceApplicationResultDto>();
            }
            
            // Try to read error message
            var errorResult = await response.Content.ReadFromJsonAsync<BuffInstanceApplicationResultDto>();
            return errorResult;
        }

        public async Task<BuffInstanceApplicationResultDto?> DeactivateBuffInstanceAsync(string characterId, string buffInstanceId)
        {
            var deactivateDto = new { CharacterId = characterId, BuffInstanceId = buffInstanceId };
            var response = await _httpClient.PostAsJsonAsync("api/buffinstances/deactivate", deactivateDto);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BuffInstanceApplicationResultDto>();
            }
            
            // Try to read error message
            var errorResult = await response.Content.ReadFromJsonAsync<BuffInstanceApplicationResultDto>();
            return errorResult;
        }

        public async Task<bool> DeleteBuffInstanceAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/buffinstances/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
