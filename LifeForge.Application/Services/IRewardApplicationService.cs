using LifeForge.Application.Models;

namespace LifeForge.Application.Services
{
    /// <summary>
    /// Service interface for applying rewards to characters
    /// </summary>
    public interface IRewardApplicationService
    {
        /// <summary>
        /// Apply rewards from a completed quest run to the character
        /// </summary>
        Task<RewardApplicationResult> ApplyQuestRewardsAsync(string questRunId);
    }
}
