using LifeForge.DataAccess.Models;

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
    }
}
