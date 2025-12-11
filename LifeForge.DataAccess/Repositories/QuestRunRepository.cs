using MongoDB.Driver;
using LifeForge.DataAccess.Configuration;
using LifeForge.DataAccess.Models;
using LifeForge.Domain;
using Microsoft.Extensions.Options;

namespace LifeForge.DataAccess.Repositories
{
    public class QuestRunRepository : IQuestRunRepository
    {
        private readonly IMongoCollection<QuestRunEntity> _questRunsCollection;

        public QuestRunRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _questRunsCollection = mongoDatabase.GetCollection<QuestRunEntity>(mongoDbSettings.Value.QuestRunsCollectionName);
        }

        public async Task<List<QuestRunEntity>> GetAllQuestRunsAsync()
        {
            return await _questRunsCollection.Find(_ => true)
                .SortByDescending(qr => qr.StartTime)
                .ToListAsync();
        }

        public async Task<List<QuestRunEntity>> GetActiveQuestRunsAsync()
        {
            return await _questRunsCollection.Find(qr => qr.Status == QuestStatus.InProgress)
                .SortByDescending(qr => qr.StartTime)
                .ToListAsync();
        }

        public async Task<QuestRunEntity?> GetQuestRunByIdAsync(string id)
        {
            return await _questRunsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<QuestRunEntity?> GetActiveQuestRunByQuestIdAsync(string questId)
        {
            return await _questRunsCollection
                .Find(qr => qr.QuestId == questId && qr.Status == QuestStatus.InProgress)
                .FirstOrDefaultAsync();
        }

        public async Task<QuestRunEntity> CreateQuestRunAsync(QuestRunEntity questRun)
        {
            questRun.CreatedAt = DateTime.UtcNow;
            questRun.UpdatedAt = DateTime.UtcNow;
            await _questRunsCollection.InsertOneAsync(questRun);
            return questRun;
        }

        public async Task<bool> UpdateQuestRunAsync(string id, QuestRunEntity questRun)
        {
            questRun.UpdatedAt = DateTime.UtcNow;
            var result = await _questRunsCollection.ReplaceOneAsync(x => x.Id == id, questRun);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteQuestRunAsync(string id)
        {
            var result = await _questRunsCollection.DeleteOneAsync(x => x.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
