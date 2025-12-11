using LifeForge.Application.Models;

namespace LifeForge.Application.Services
{
    public interface IBuffInstanceApplicationService
    {
        Task<BuffInstanceApplicationResult> ActivateBuffAsync(string characterId, string buffId);
        Task<BuffInstanceApplicationResult> DeactivateBuffInstanceAsync(string characterId, string buffInstanceId);
    }
}
