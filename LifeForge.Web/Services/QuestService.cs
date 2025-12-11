using System.Net.Http.Json;
using LifeForge.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace LifeForge.Web.Services
{
    public class QuestService
    {
        private readonly HttpClient _httpClient;

        public QuestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<QuestDto>> GetAllQuestsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<QuestDto>>("api/quests") ?? new List<QuestDto>();
        }

        public async Task<QuestDto?> GetQuestAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<QuestDto>($"api/quests/{id}");
        }

        public async Task<QuestDto?> CreateQuestAsync(QuestDto quest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/quests", quest);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuestDto>();
        }

        public async Task<bool> UpdateQuestAsync(string id, QuestDto quest)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/quests/{id}", quest);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteQuestAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/quests/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<ImageUploadResult?> UploadImageAsync(IBrowserFile file)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/quests/upload-image", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ImageUploadResult>();
            }
            return null;
        }

        public class ImageUploadResult
        {
            public string FileName { get; set; } = string.Empty;
            public string ImageData { get; set; } = string.Empty;
            public string ContentType { get; set; } = string.Empty;
        }
    }
}
