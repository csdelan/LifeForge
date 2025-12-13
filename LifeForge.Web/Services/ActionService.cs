using System.Net.Http.Json;
using LifeForge.Web.Models;

namespace LifeForge.Web.Services
{
    public class ActionService
    {
        private readonly HttpClient _httpClient;

        public ActionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ActionDto>> GetAllActionsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<ActionDto>>("api/actions");
            return result ?? new List<ActionDto>();
        }

        public async Task<ActionDto?> GetActionByIdAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<ActionDto>($"api/actions/{id}");
        }

        public async Task<ActionResultDto?> PerformActionAsync(string characterId, string actionId)
        {
            var performDto = new
            {
                CharacterId = characterId,
                ActionId = actionId
            };

            var response = await _httpClient.PostAsJsonAsync("api/actions/perform", performDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ActionResultDto>();
        }
    }
}
