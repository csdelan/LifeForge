using MongoDB.Driver;
using LifeForge.DataAccess.Configuration;
using LifeForge.DataAccess.Models;
using Microsoft.Extensions.Options;

namespace LifeForge.DataAccess.Repositories
{
    public class BuffRepository : IBuffRepository
    {
        private readonly IMongoCollection<BuffEntity> _buffsCollection;

        public BuffRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _buffsCollection = mongoDatabase.GetCollection<BuffEntity>(mongoDbSettings.Value.BuffsCollectionName);
        }

        public async Task<List<BuffEntity>> GetAllBuffsAsync()
        {
            return await _buffsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<BuffEntity?> GetBuffByIdAsync(string id)
        {
            return await _buffsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<BuffEntity> CreateBuffAsync(BuffEntity buff)
        {
            buff.CreatedAt = DateTime.UtcNow;
            buff.UpdatedAt = DateTime.UtcNow;
            await _buffsCollection.InsertOneAsync(buff);
            return buff;
        }

        public async Task<bool> UpdateBuffAsync(string id, BuffEntity buff)
        {
            buff.UpdatedAt = DateTime.UtcNow;
            var result = await _buffsCollection.ReplaceOneAsync(x => x.Id == id, buff);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteBuffAsync(string id)
        {
            var result = await _buffsCollection.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
