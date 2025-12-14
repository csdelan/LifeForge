using LifeForge.Domain;

namespace LifeForge.Application.Services
{
    public interface IBuffAggregationService
    {
        /// <summary>
        /// Calculates the aggregate modifiers from all active buffs for a character
        /// </summary>
        Task<AggregateModifier> CalculateAggregateModifiersAsync(string characterId);

        /// <summary>
        /// Updates the character's ActiveBuffModifiers based on current active buffs
        /// </summary>
        Task UpdateCharacterAggregateModifiersAsync(string characterId);
    }
}
