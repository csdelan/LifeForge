using MongoDB.Driver;
using LifeForge.DataAccess.Configuration;
using LifeForge.DataAccess.Models;
using LifeForge.Domain;
using Microsoft.Extensions.Options;

namespace LifeForge.DataAccess.Repositories
{
    public class ActionRepository : IActionRepository
    {
        private readonly IMongoCollection<ActionEntity> _actionsCollection;

        public ActionRepository(IOptions<MongoDbSettings> mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _actionsCollection = mongoDatabase.GetCollection<ActionEntity>(mongoDbSettings.Value.ActionsCollectionName);
        }

        public async Task<List<ActionEntity>> GetAllActionsAsync()
        {
            return await _actionsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<ActionEntity?> GetActionByIdAsync(string id)
        {
            return await _actionsCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<ActionEntity>> GetActionsByCategoryAsync(ActionCategory category)
        {
            return await _actionsCollection.Find(a => a.Category == category).ToListAsync();
        }

        public async Task<ActionEntity> CreateActionAsync(ActionEntity action)
        {
            action.CreatedAt = DateTime.UtcNow;
            action.UpdatedAt = DateTime.UtcNow;
            await _actionsCollection.InsertOneAsync(action);
            return action;
        }

        public async Task<bool> UpdateActionAsync(string id, ActionEntity action)
        {
            action.UpdatedAt = DateTime.UtcNow;
            var result = await _actionsCollection.ReplaceOneAsync(a => a.Id == id, action);
            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteActionAsync(string id)
        {
            var result = await _actionsCollection.DeleteOneAsync(a => a.Id == id);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
