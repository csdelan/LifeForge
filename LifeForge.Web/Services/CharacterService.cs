using System.Net.Http.Json;
using LifeForge.Web.Models;

namespace LifeForge.Web.Services
{
    public class CharacterService
    {
        private readonly HttpClient _httpClient;

        public CharacterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CharacterDto?> GetCharacterAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CharacterDto>("api/characters");
            }
            catch (HttpRequestException)
            {
                // Character doesn't exist yet
                return null;
            }
        }
    }
}
