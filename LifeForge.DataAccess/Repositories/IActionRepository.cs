using LifeForge.DataAccess.Models;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Repositories
{
    public interface IActionRepository
    {
        Task<List<ActionEntity>> GetAllActionsAsync();
        Task<ActionEntity?> GetActionByIdAsync(string id);
        Task<List<ActionEntity>> GetActionsByCategoryAsync(ActionCategory category);
        Task<ActionEntity> CreateActionAsync(ActionEntity action);
        Task<bool> UpdateActionAsync(string id, ActionEntity action);
        Task<bool> DeleteActionAsync(string id);
    }
}
