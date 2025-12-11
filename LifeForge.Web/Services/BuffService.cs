using System.Net.Http.Json;
using LifeForge.Web.Models;
using Microsoft.AspNetCore.Components.Forms;

namespace LifeForge.Web.Services
{
    public class BuffService
    {
        private readonly HttpClient _httpClient;

        public BuffService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BuffDto>> GetAllBuffsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<BuffDto>>("api/buffs") ?? new List<BuffDto>();
        }

        public async Task<BuffDto?> GetBuffAsync(string id)
        {
            return await _httpClient.GetFromJsonAsync<BuffDto>($"api/buffs/{id}");
        }

        public async Task<BuffDto?> CreateBuffAsync(BuffDto buff)
        {
            var response = await _httpClient.PostAsJsonAsync("api/buffs", buff);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BuffDto>();
        }

        public async Task<bool> UpdateBuffAsync(string id, BuffDto buff)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/buffs/{id}", buff);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteBuffAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"api/buffs/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<ImageUploadResult?> UploadImageAsync(IBrowserFile file)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.Name);

            var response = await _httpClient.PostAsync("api/buffs/upload-image", content);
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
