using LifeForge.DataAccess.Repositories;
using LifeForge.Domain;
using Microsoft.Extensions.Logging;

namespace LifeForge.Application.Services
{
    public class BuffAggregationService : IBuffAggregationService
    {
        private readonly IBuffInstanceRepository _buffInstanceRepository;
        private readonly IBuffRepository _buffRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly ILogger<BuffAggregationService> _logger;

        public BuffAggregationService(
            IBuffInstanceRepository buffInstanceRepository,
            IBuffRepository buffRepository,
            ICharacterRepository characterRepository,
            ILogger<BuffAggregationService> logger)
        {
            _buffInstanceRepository = buffInstanceRepository;
            _buffRepository = buffRepository;
            _characterRepository = characterRepository;
            _logger = logger;
        }

        public async Task<AggregateModifier> CalculateAggregateModifiersAsync(string characterId)
        {
            var aggregate = new AggregateModifier();

            // Get all buff instances with Active status for this character
            var buffInstances = await _buffInstanceRepository.GetActiveBuffInstancesByCharacterIdAsync(characterId);
            var activeBuffs = buffInstances.Where(bi => bi.Status == BuffInstanceStatus.Active).ToList();

            // Group by BuffId to handle MaxStacks logic
            var buffGroups = activeBuffs.GroupBy(bi => bi.BuffId);

            foreach (var buffGroup in buffGroups)
            {
                // Get the buff definition to check MaxStacks
                var buff = await _buffRepository.GetBuffByIdAsync(buffGroup.Key);
                if (buff == null)
                {
                    _logger.LogWarning("Buff {BuffId} not found when calculating aggregates", buffGroup.Key);
                    continue;
                }

                // Sort by StartTime (oldest first) and take only up to MaxStacks
                var effectiveInstances = buffGroup
                    .OrderBy(bi => bi.StartTime)
                    .Take(buff.MaxStacks)
                    .ToList();

                // Apply each effective instance's modifiers
                foreach (var instance in effectiveInstances)
                {
                    // Each instance contributes based on its stacks value
                    // (though typically stacks=1 for independent instances)
                    for (int i = 0; i < instance.Stacks; i++)
                    {
                        aggregate.HPModifier += instance.HPModifier;
                        aggregate.HPMaxModifier += instance.HPMaxModifier;
                        aggregate.HPPercentModifier += instance.HPPercentModifier;
                        aggregate.HPMaxPercentModifier += instance.HPMaxPercentModifier;
                        aggregate.MPModifier += instance.MPModifier;
                        aggregate.MPMaxModifier += instance.MPMaxModifier;
                        aggregate.MPPercentModifier += instance.MPPercentModifier;
                        aggregate.MPMaxPercentModifier += instance.MPMaxPercentModifier;
                        aggregate.XpGainsPercentModifier += instance.XpGainsPercentModifier;
                    }
                }

                _logger.LogDebug("Applied {Count} instances of buff {BuffName} (max {MaxStacks})",
                    effectiveInstances.Count, buff.Name, buff.MaxStacks);
            }

            return aggregate;
        }

        public async Task UpdateCharacterAggregateModifiersAsync(string characterId)
        {
            var character = await _characterRepository.GetCharacterByIdAsync(characterId);
            if (character == null)
            {
                _logger.LogWarning("Character {CharacterId} not found when updating aggregate modifiers", characterId);
                return;
            }

            var aggregate = await CalculateAggregateModifiersAsync(characterId);

            // Convert to entity and update
            character.ActiveBuffModifiers = DataAccess.Models.AggregateModifierEntity.FromDomain(aggregate);
            await _characterRepository.UpdateCharacterAsync(characterId, character);

            _logger.LogInformation("Updated aggregate modifiers for character {CharacterId}: HP+{HP}, HPMax+{HPMax}, MP+{MP}, MPMax+{MPMax}, XP+{XP}%",
                characterId, aggregate.HPModifier, aggregate.HPMaxModifier, aggregate.MPModifier, aggregate.MPMaxModifier, aggregate.XpGainsPercentModifier);
        }
    }
}
