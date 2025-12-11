using LifeForge.DataAccess.Models;

namespace LifeForge.DataAccess.Repositories
{
    public interface IQuestRepository
    {
        Task<List<QuestEntity>> GetAllQuestsAsync();
        Task<QuestEntity?> GetQuestByIdAsync(string id);
        Task<QuestEntity> CreateQuestAsync(QuestEntity quest);
        Task<bool> UpdateQuestAsync(string id, QuestEntity quest);
        Task<bool> DeleteQuestAsync(string id);
    }
}
