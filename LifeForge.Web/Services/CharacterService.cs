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

        public async Task<List<CharacterDto>> GetAllCharactersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CharacterDto>>("api/characters/all") ?? new List<CharacterDto>();
            }
            catch (HttpRequestException)
            {
                // No characters exist yet
                return new List<CharacterDto>();
            }
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
