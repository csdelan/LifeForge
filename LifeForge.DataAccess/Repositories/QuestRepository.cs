using MongoDB.Driver;
using LifeForge.DataAccess.Configuration;
using LifeForge.DataAccess.Models;
using Microsoft.Extensions.Options;

namespace LifeForge.DataAccess.Repositories
{
    public class QuestRepository : IQuestRepository
    {
        private readonly IMongoCollection<QuestEntity> _questsCollection;

        public QuestRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _questsCollection = mongoDatabase.GetCollection<QuestEntity>(mongoDbSettings.Value.QuestsCollectionName);
        }

        public async Task<List<QuestEntity>> GetAllQuestsAsync()
        {
            return await _questsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<QuestEntity?> GetQuestByIdAsync(string id)
        {
            return await _questsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<QuestEntity> CreateQuestAsync(QuestEntity quest)
        {
            quest.CreatedAt = DateTime.UtcNow;
            quest.UpdatedAt = DateTime.UtcNow;
            await _questsCollection.InsertOneAsync(quest);
            return quest;
        }

        public async Task<bool> UpdateQuestAsync(string id, QuestEntity quest)
        {
            quest.UpdatedAt = DateTime.UtcNow;
            var result = await _questsCollection.ReplaceOneAsync(x => x.Id == id, quest);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteQuestAsync(string id)
        {
            var result = await _questsCollection.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
