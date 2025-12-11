using LifeForge.DataAccess.Models;

namespace LifeForge.DataAccess.Repositories
{
    public interface IQuestRunRepository
    {
        Task<List<QuestRunEntity>> GetAllQuestRunsAsync();
        Task<List<QuestRunEntity>> GetActiveQuestRunsAsync();
        Task<QuestRunEntity?> GetQuestRunByIdAsync(string id);
        Task<QuestRunEntity?> GetActiveQuestRunByQuestIdAsync(string questId);
        Task<QuestRunEntity> CreateQuestRunAsync(QuestRunEntity questRun);
        Task<bool> UpdateQuestRunAsync(string id, QuestRunEntity questRun);
        Task<bool> DeleteQuestRunAsync(string id);
    }
}
