using MongoDB.Driver;
using LifeForge.DataAccess.Configuration;
using LifeForge.DataAccess.Models;
using Microsoft.Extensions.Options;

namespace LifeForge.DataAccess.Repositories
{
    public class BuffInstanceRepository : IBuffInstanceRepository
    {
        private readonly IMongoCollection<BuffInstanceEntity> _buffInstancesCollection;

        public BuffInstanceRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _buffInstancesCollection = mongoDatabase.GetCollection<BuffInstanceEntity>(mongoDbSettings.Value.BuffInstancesCollectionName);
        }

        public async Task<List<BuffInstanceEntity>> GetAllBuffInstancesAsync()
        {
            return await _buffInstancesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<List<BuffInstanceEntity>> GetActiveBuffInstancesByCharacterIdAsync(string characterId)
        {
            return await _buffInstancesCollection
                .Find(x => x.CharacterId == characterId && x.IsActive)
                .ToListAsync();
        }

        public async Task<BuffInstanceEntity?> GetBuffInstanceByIdAsync(string id)
        {
            return await _buffInstancesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<BuffInstanceEntity> CreateBuffInstanceAsync(BuffInstanceEntity buffInstance)
        {
            buffInstance.CreatedAt = DateTime.UtcNow;
            buffInstance.UpdatedAt = DateTime.UtcNow;
            await _buffInstancesCollection.InsertOneAsync(buffInstance);
            return buffInstance;
        }

        public async Task<bool> UpdateBuffInstanceAsync(string id, BuffInstanceEntity buffInstance)
        {
            buffInstance.UpdatedAt = DateTime.UtcNow;
            var result = await _buffInstancesCollection.ReplaceOneAsync(x => x.Id == id, buffInstance);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteBuffInstanceAsync(string id)
        {
            var result = await _buffInstancesCollection.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public async Task<bool> DeactivateBuffInstanceAsync(string id)
        {
            var update = Builders<BuffInstanceEntity>.Update
                .Set(x => x.IsActive, false)
                .Set(x => x.UpdatedAt, DateTime.UtcNow);

            var result = await _buffInstancesCollection.UpdateOneAsync(x => x.Id == id, update);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }
    }
}
