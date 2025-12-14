using System.Net.Http.Json;
using LifeForge.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

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

        public async Task<ActionDto?> CreateActionAsync(ActionDto action)
        {
            var response = await _httpClient.PostAsJsonAsync("api/actions", action);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ActionDto>();
        }

        public async Task<bool> UpdateActionAsync(string id, ActionDto action)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/actions/{id}", action);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteActionAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/actions/{id}");
            return response.IsSuccessStatusCode;
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

        public async Task<ImageUploadResult?> UploadImageAsync(IBrowserFile file)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/actions/upload-image", content);
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
