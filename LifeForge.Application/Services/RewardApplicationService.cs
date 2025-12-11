using Microsoft.Extensions.Logging;
using LifeForge.Application.Models;
using LifeForge.DataAccess.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.Domain;

namespace LifeForge.Application.Services
{
    public class RewardApplicationService : IRewardApplicationService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IQuestRunRepository _questRunRepository;
        private readonly ILogger<RewardApplicationService> _logger;

        public RewardApplicationService(
            ICharacterRepository characterRepository,
            IQuestRunRepository questRunRepository,
            ILogger<RewardApplicationService> logger)
        {
            _characterRepository = characterRepository;
            _questRunRepository = questRunRepository;
            _logger = logger;
        }

        public async Task<RewardApplicationResult> ApplyQuestRewardsAsync(string questRunId)
        {
            var result = new RewardApplicationResult();

            try
            {
                // 1. Get quest run
                var questRun = await _questRunRepository.GetQuestRunByIdAsync(questRunId);
                if (questRun == null)
                {
                    result.ErrorMessage = "Quest run not found";
                    return result;
                }

                // 2. Verify quest is completed
                if (questRun.Status != QuestStatus.Completed)
                {
                    result.ErrorMessage = "Quest must be completed to apply rewards";
                    return result;
                }

                // 3. Get or create character
                var characterEntity = await _characterRepository.GetCharacterAsync();
                if (characterEntity == null)
                {
                    // Create default character if none exists
                    _logger.LogInformation("No character found, creating default character");
                    characterEntity = CreateDefaultCharacter();
                    await _characterRepository.CreateCharacterAsync(characterEntity);
                }

                var character = characterEntity.ToDomain();

                // 4. Apply each reward
                foreach (var rewardEntity in questRun.Rewards)
                {
                    var reward = new Reward(
                        rewardEntity.Type,
                        rewardEntity.RewardClass,
                        rewardEntity.Amount,
                        rewardEntity.Icon ?? "??"
                    );

                    ApplyReward(character, reward, result);
                }

                // 5. Save updated character
                var updatedEntity = CharacterEntity.FromDomain(character);
                updatedEntity.Id = characterEntity.Id;
                await _characterRepository.UpdateCharacterAsync(updatedEntity);

                result.Success = true;
                _logger.LogInformation("Successfully applied {RewardCount} rewards from quest run {QuestRunId}",
                    questRun.Rewards.Count, questRunId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying rewards for quest run {QuestRunId}", questRunId);
                result.ErrorMessage = $"Error applying rewards: {ex.Message}";
                return result;
            }
        }

        private void ApplyReward(Character character, Reward reward, RewardApplicationResult result)
        {
            try
            {
                character.ApplyReward(reward);

                // Track what was applied
                switch (reward.Type)
                {
                    case RewardType.Currency:
                        if (!result.CurrenciesGained.ContainsKey(reward.RewardClass))
                            result.CurrenciesGained[reward.RewardClass] = 0;
                        result.CurrenciesGained[reward.RewardClass] += reward.Amount;
                        result.AppliedRewards.Add($"+{reward.Amount} {reward.RewardClass}");
                        break;

                    case RewardType.Experience:
                        if (!result.ExperienceGained.ContainsKey(reward.RewardClass))
                            result.ExperienceGained[reward.RewardClass] = 0;
                        result.ExperienceGained[reward.RewardClass] += reward.Amount;
                        result.AppliedRewards.Add($"+{reward.Amount} {reward.RewardClass} XP");
                        break;
                }

                _logger.LogDebug("Applied reward: {Type} {Amount} {Class}",
                    reward.Type, reward.Amount, reward.RewardClass);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying individual reward: {Type} {Class} {Amount}",
                    reward.Type, reward.RewardClass, reward.Amount);
                throw;
            }
        }

        private CharacterEntity CreateDefaultCharacter()
        {
            var character = new Character("Hero of LifeForge")
            {
                HP = 100,
                HPMax = 100,
                MP = 100,
                MPMax = 100,
                Strength = 10,
                Discipline = 10,
                Focus = 10
            };

            // Initialize with starting currencies
            character.Currencies[CurrencyType.Gold] = 0;
            character.Currencies[CurrencyType.Karma] = 0;
            character.Currencies[CurrencyType.DesignWorkslot] = 0;

            return CharacterEntity.FromDomain(character);
        }
    }
}
