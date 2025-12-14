using LifeForge.Application.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.DataAccess.Models;
using LifeForge.Domain;

namespace LifeForge.Application.Services
{
    public class BuffInstanceApplicationService : IBuffInstanceApplicationService
    {
        private readonly IBuffInstanceRepository _buffInstanceRepository;
        private readonly IBuffRepository _buffRepository;
        private readonly ICharacterRepository _characterRepository;
        private readonly IBuffAggregationService _buffAggregationService;

        public BuffInstanceApplicationService(
            IBuffInstanceRepository buffInstanceRepository,
            IBuffRepository buffRepository,
            ICharacterRepository characterRepository,
            IBuffAggregationService buffAggregationService)
        {
            _buffInstanceRepository = buffInstanceRepository;
            _buffRepository = buffRepository;
            _characterRepository = characterRepository;
            _buffAggregationService = buffAggregationService;
        }

        public async Task<BuffInstanceApplicationResult> ActivateBuffAsync(string characterId, string buffId)
        {
            var result = new BuffInstanceApplicationResult();

            try
            {
                // 1. Get the buff definition
                var buff = await _buffRepository.GetBuffByIdAsync(buffId);
                if (buff == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Buff not found";
                    return result;
                }

                // 2. Get the character
                var character = await _characterRepository.GetCharacterByIdAsync(characterId);
                if (character == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Character not found";
                    return result;
                }

                // 3. Create new buff instance with Pending status
                // It will be activated at next midnight job run
                var startTime = DateTime.UtcNow;
                var endTime = startTime.AddDays(buff.DurationDays);

                var buffInstance = new BuffInstanceEntity
                {
                    BuffId = buffId,
                    CharacterId = characterId,
                    BuffName = buff.Name,
                    Description = buff.Description,
                    IsDebuff = buff.IsDebuff,
                    StartTime = startTime,
                    EndTime = endTime,
                    Stacks = 1,
                    IsActive = true,
                    Status = BuffInstanceStatus.Pending,
                    HPModifier = buff.HPModifier,
                    HPMaxModifier = buff.HPMaxModifier,
                    HPPercentModifier = buff.HPPercentModifier,
                    HPMaxPercentModifier = buff.HPMaxPercentModifier,
                    MPModifier = buff.MPModifier,
                    MPMaxModifier = buff.MPMaxModifier,
                    MPPercentModifier = buff.MPPercentModifier,
                    MPMaxPercentModifier = buff.MPMaxPercentModifier,
                    XpGainsPercentModifier = buff.XpGainsPercentModifier,
                    ImageData = buff.ImageData,
                    ImageContentType = buff.ImageContentType
                };

                var createdInstance = await _buffInstanceRepository.CreateBuffInstanceAsync(buffInstance);

                result.Success = true;
                result.BuffInstanceId = createdInstance.Id;
                result.ModifiersApplied = GetModifiersDictionary(buff);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error activating buff: {ex.Message}";
                return result;
            }
        }

        public async Task<BuffInstanceApplicationResult> DeactivateBuffInstanceAsync(string characterId, string buffInstanceId)
        {
            var result = new BuffInstanceApplicationResult();

            try
            {
                // 1. Get the buff instance
                var buffInstance = await _buffInstanceRepository.GetBuffInstanceByIdAsync(buffInstanceId);
                if (buffInstance == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Buff instance not found";
                    return result;
                }

                if (buffInstance.CharacterId != characterId)
                {
                    result.Success = false;
                    result.ErrorMessage = "Buff instance does not belong to this character";
                    return result;
                }

                // 2. Get the character
                var character = await _characterRepository.GetCharacterByIdAsync(characterId);
                if (character == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Character not found";
                    return result;
                }

                // 3. Delete the buff instance (manual deactivation is immediate)
                await _buffInstanceRepository.DeleteBuffInstanceAsync(buffInstanceId);

                // 4. Immediately recalculate aggregate modifiers (manual operations trigger immediate update)
                await _buffAggregationService.UpdateCharacterAggregateModifiersAsync(characterId);

                result.Success = true;
                result.BuffInstanceId = buffInstanceId;
                result.ModifiersApplied = GetModifiersDictionaryFromInstance(buffInstance);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error deactivating buff: {ex.Message}";
                return result;
            }
        }

        private Dictionary<string, int> GetModifiersDictionary(BuffEntity buff)
        {
            var modifiers = new Dictionary<string, int>();
            if (buff.HPModifier != 0) modifiers["HP"] = buff.HPModifier;
            if (buff.HPMaxModifier != 0) modifiers["HP Max"] = buff.HPMaxModifier;
            if (buff.HPPercentModifier != 0) modifiers["HP %"] = buff.HPPercentModifier;
            if (buff.HPMaxPercentModifier != 0) modifiers["HP Max %"] = buff.HPMaxPercentModifier;
            if (buff.MPModifier != 0) modifiers["MP"] = buff.MPModifier;
            if (buff.MPMaxModifier != 0) modifiers["MP Max"] = buff.MPMaxModifier;
            if (buff.MPPercentModifier != 0) modifiers["MP %"] = buff.MPPercentModifier;
            if (buff.MPMaxPercentModifier != 0) modifiers["MP Max %"] = buff.MPMaxPercentModifier;
            if (buff.XpGainsPercentModifier != 0) modifiers["XP Gains %"] = buff.XpGainsPercentModifier;
            return modifiers;
        }

        private Dictionary<string, int> GetModifiersDictionaryFromInstance(BuffInstanceEntity buffInstance)
        {
            var modifiers = new Dictionary<string, int>();
            if (buffInstance.HPModifier != 0) modifiers["HP"] = buffInstance.HPModifier;
            if (buffInstance.HPMaxModifier != 0) modifiers["HP Max"] = buffInstance.HPMaxModifier;
            if (buffInstance.HPPercentModifier != 0) modifiers["HP %"] = buffInstance.HPPercentModifier;
            if (buffInstance.HPMaxPercentModifier != 0) modifiers["HP Max %"] = buffInstance.HPMaxPercentModifier;
            if (buffInstance.MPModifier != 0) modifiers["MP"] = buffInstance.MPModifier;
            if (buffInstance.MPMaxModifier != 0) modifiers["MP Max"] = buffInstance.MPMaxModifier;
            if (buffInstance.MPPercentModifier != 0) modifiers["MP %"] = buffInstance.MPPercentModifier;
            if (buffInstance.MPMaxPercentModifier != 0) modifiers["MP Max %"] = buffInstance.MPMaxPercentModifier;
            if (buffInstance.XpGainsPercentModifier != 0) modifiers["XP Gains %"] = buffInstance.XpGainsPercentModifier;
            return modifiers;
        }
    }
}
