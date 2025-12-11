using MongoDB.Driver;
using LifeForge.DataAccess.Configuration;
using LifeForge.DataAccess.Models;
using Microsoft.Extensions.Options;

namespace LifeForge.DataAccess.Repositories
{
    public class CharacterRepository : ICharacterRepository
    {
        private readonly IMongoCollection<CharacterEntity> _charactersCollection;

        public CharacterRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _charactersCollection = mongoDatabase.GetCollection<CharacterEntity>(
                mongoDbSettings.Value.CharactersCollectionName);
        }

        public async Task<CharacterEntity?> GetCharacterAsync()
        {
            // Single character system - get the first (and only) character
            return await _charactersCollection.Find(_ => true).FirstOrDefaultAsync();
        }

        public async Task<List<CharacterEntity>> GetAllCharactersAsync()
        {
            return await _charactersCollection.Find(_ => true).ToListAsync();
        }

        public async Task<CharacterEntity?> GetCharacterByIdAsync(string id)
        {
            return await _charactersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<CharacterEntity> CreateCharacterAsync(CharacterEntity character)
        {
            character.CreatedAt = DateTime.UtcNow;
            character.UpdatedAt = DateTime.UtcNow;
            await _charactersCollection.InsertOneAsync(character);
            return character;
        }

        public async Task<bool> UpdateCharacterAsync(CharacterEntity character)
        {
            if (string.IsNullOrEmpty(character.Id))
            {
                // If no ID, this might be a new character or first update
                var existing = await GetCharacterAsync();
                if (existing != null)
                {
                    character.Id = existing.Id;
                }
                else
                {
                    // Create new character
                    await CreateCharacterAsync(character);
                    return true;
                }
            }

            character.UpdatedAt = DateTime.UtcNow;
            var result = await _charactersCollection.ReplaceOneAsync(
                x => x.Id == character.Id,
                character);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateCharacterAsync(string id, CharacterEntity character)
        {
            character.UpdatedAt = DateTime.UtcNow;
            var result = await _charactersCollection.ReplaceOneAsync(
                x => x.Id == id,
                character);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> CharacterExistsAsync()
        {
            var count = await _charactersCollection.CountDocumentsAsync(_ => true);
            return count > 0;
        }
    }
}
