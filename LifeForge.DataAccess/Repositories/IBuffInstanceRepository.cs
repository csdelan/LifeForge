using LifeForge.DataAccess.Models;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Repositories
{
    public interface IBuffInstanceRepository
    {
        Task<List<BuffInstanceEntity>> GetAllBuffInstancesAsync();
        Task<List<BuffInstanceEntity>> GetActiveBuffInstancesByCharacterIdAsync(string characterId);
        Task<BuffInstanceEntity?> GetBuffInstanceByIdAsync(string id);
        Task<BuffInstanceEntity> CreateBuffInstanceAsync(BuffInstanceEntity buffInstance);
        Task<bool> UpdateBuffInstanceAsync(string id, BuffInstanceEntity buffInstance);
        Task<bool> DeleteBuffInstanceAsync(string id);
        Task<bool> DeactivateBuffInstanceAsync(string id);

        /// <summary>
        /// Get all buff instances with Pending status
        /// </summary>
        Task<List<BuffInstanceEntity>> GetPendingBuffInstancesAsync();

        /// <summary>
        /// Get all buff instances with Active status that have expired (past EndTime)
        /// </summary>
        Task<List<BuffInstanceEntity>> GetExpiredBuffInstancesAsync();

        /// <summary>
        /// Bulk update status for multiple buff instances
        /// </summary>
        Task<bool> BulkUpdateStatusAsync(List<string> buffInstanceIds, BuffInstanceStatus newStatus);
    }
}
