using LifeForge.DataAccess.Models;

namespace LifeForge.DataAccess.Repositories
{
    public interface IBuffRepository
    {
        Task<List<BuffEntity>> GetAllBuffsAsync();
        Task<BuffEntity?> GetBuffByIdAsync(string id);
        Task<BuffEntity> CreateBuffAsync(BuffEntity buff);
        Task<bool> UpdateBuffAsync(string id, BuffEntity buff);
        Task<bool> DeleteBuffAsync(string id);
    }
}
