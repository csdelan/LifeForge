using System.Net.Http.Json;
using LifeForge.Web.Models;

namespace LifeForge.Web.Services
{
    public class QuestRunService
    {
        private readonly HttpClient _httpClient;

        public QuestRunService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<QuestRunDto>> GetAllQuestRunsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<QuestRunDto>>("api/questruns") ?? new List<QuestRunDto>();
        }

        public async Task<List<QuestRunDto>> GetActiveQuestRunsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<QuestRunDto>>("api/questruns/active") ?? new List<QuestRunDto>();
        }

        public async Task<QuestRunDto?> GetQuestRunAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<QuestRunDto>($"api/questruns/{id}");
        }

        public async Task<QuestRunDto?> StartQuestRunAsync(string questId)
        {
            var startDto = new StartQuestRunDto { QuestId = questId };
            var response = await _httpClient.PostAsJsonAsync("api/questruns/start", startDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuestRunDto>();
        }

        public async Task<QuestRunDto?> CompleteQuestRunAsync(string questRunId)
        {
            var response = await _httpClient.PostAsync($"api/questruns/{questRunId}/complete", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuestRunDto>();
        }

        public async Task<bool> DeleteQuestRunAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/questruns/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
