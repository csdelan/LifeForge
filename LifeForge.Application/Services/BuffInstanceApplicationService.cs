using LifeForge.Application.Models;
using LifeForge.DataAccess.Repositories;
using LifeForge.DataAccess.Models;

namespace LifeForge.Application.Services
{
    public class BuffInstanceApplicationService : IBuffInstanceApplicationService
    {
        private readonly IBuffInstanceRepository _buffInstanceRepository;
        private readonly IBuffRepository _buffRepository;
        private readonly ICharacterRepository _characterRepository;

        public BuffInstanceApplicationService(
            IBuffInstanceRepository buffInstanceRepository,
            IBuffRepository buffRepository,
            ICharacterRepository characterRepository)
        {
            _buffInstanceRepository = buffInstanceRepository;
            _buffRepository = buffRepository;
            _characterRepository = characterRepository;
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

                // 3. Check if buff can stack
                var existingBuffs = await _buffInstanceRepository.GetActiveBuffInstancesByCharacterIdAsync(characterId);
                var existingBuff = existingBuffs.FirstOrDefault(b => b.BuffId == buffId && b.IsActive);
                
                if (existingBuff != null)
                {
                    // Check if we can stack
                    if (existingBuff.Stacks < buff.MaxStacks)
                    {
                        // Increment stacks
                        existingBuff.Stacks++;
                        existingBuff.UpdatedAt = DateTime.UtcNow;
                        await _buffInstanceRepository.UpdateBuffInstanceAsync(existingBuff.Id!, existingBuff);
                        
                        // Apply modifiers again for the new stack
                        await ApplyBuffModifiersToCharacter(character, buff, existingBuff.Stacks);
                        
                        result.Success = true;
                        result.BuffInstanceId = existingBuff.Id;
                        result.ModifiersApplied = GetModifiersDictionary(buff);
                        return result;
                    }
                    else
                    {
                        result.Success = false;
                        result.ErrorMessage = $"Buff '{buff.Name}' is already at maximum stacks ({buff.MaxStacks})";
                        return result;
                    }
                }

                // 4. Create new buff instance
                var startTime = DateTime.UtcNow;
                var endTime = startTime.AddDays(buff.DurationDays);

                var buffInstance = new BuffInstanceEntity
                {
                    BuffId = buffId,
                    CharacterId = characterId,
                    BuffName = buff.Name,
                    IsDebuff = buff.IsDebuff,
                    StartTime = startTime,
                    EndTime = endTime,
                    Stacks = 1,
                    IsActive = true,
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

                // 5. Apply buff modifiers to character
                await ApplyBuffModifiersToCharacter(character, buff, 1);

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

                // 3. Remove buff modifiers from character
                await RemoveBuffModifiersFromCharacter(character, buffInstance);

                // 4. Deactivate the buff instance
                await _buffInstanceRepository.DeactivateBuffInstanceAsync(buffInstanceId);

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

        private async Task ApplyBuffModifiersToCharacter(CharacterEntity character, BuffEntity buff, int stacks)
        {
            // Calculate base values for percentage modifiers
            int baseHPMax = character.HPMax;
            int baseMPMax = character.MPMax;

            // Apply flat modifiers (multiplied by stacks)
            character.HP += buff.HPModifier * stacks;
            character.HPMax += buff.HPMaxModifier * stacks;
            character.MP += buff.MPModifier * stacks;
            character.MPMax += buff.MPMaxModifier * stacks;

            // Apply percentage modifiers (multiplied by stacks)
            if (buff.HPPercentModifier != 0)
            {
                int hpChange = (int)((character.HP * buff.HPPercentModifier / 100.0) * stacks);
                character.HP += hpChange;
            }

            if (buff.HPMaxPercentModifier != 0)
            {
                int hpMaxChange = (int)((baseHPMax * buff.HPMaxPercentModifier / 100.0) * stacks);
                character.HPMax += hpMaxChange;
            }

            if (buff.MPPercentModifier != 0)
            {
                int mpChange = (int)((character.MP * buff.MPPercentModifier / 100.0) * stacks);
                character.MP += mpChange;
            }

            if (buff.MPMaxPercentModifier != 0)
            {
                int mpMaxChange = (int)((baseMPMax * buff.MPMaxPercentModifier / 100.0) * stacks);
                character.MPMax += mpMaxChange;
            }

            // Ensure HP/MP don't exceed max or go below 0
            character.HP = Math.Max(0, Math.Min(character.HP, character.HPMax));
            character.MP = Math.Max(0, Math.Min(character.MP, character.MPMax));

            // Update character in database
            await _characterRepository.UpdateCharacterAsync(character.Id!, character);
        }

        private async Task RemoveBuffModifiersFromCharacter(CharacterEntity character, BuffInstanceEntity buffInstance)
        {
            // Calculate base values for percentage modifiers (before removal)
            int baseHPMax = character.HPMax;
            int baseMPMax = character.MPMax;

            int stacks = buffInstance.Stacks;

            // Remove flat modifiers (multiplied by stacks)
            character.HP -= buffInstance.HPModifier * stacks;
            character.HPMax -= buffInstance.HPMaxModifier * stacks;
            character.MP -= buffInstance.MPModifier * stacks;
            character.MPMax -= buffInstance.MPMaxModifier * stacks;

            // Remove percentage modifiers (multiplied by stacks)
            if (buffInstance.HPPercentModifier != 0)
            {
                int hpChange = (int)((character.HP * buffInstance.HPPercentModifier / 100.0) * stacks);
                character.HP -= hpChange;
            }

            if (buffInstance.HPMaxPercentModifier != 0)
            {
                int hpMaxChange = (int)((baseHPMax * buffInstance.HPMaxPercentModifier / 100.0) * stacks);
                character.HPMax -= hpMaxChange;
            }

            if (buffInstance.MPPercentModifier != 0)
            {
                int mpChange = (int)((character.MP * buffInstance.MPPercentModifier / 100.0) * stacks);
                character.MP -= mpChange;
            }

            if (buffInstance.MPMaxPercentModifier != 0)
            {
                int mpMaxChange = (int)((baseMPMax * buffInstance.MPMaxPercentModifier / 100.0) * stacks);
                character.MPMax -= mpMaxChange;
            }

            // Ensure HP/MP don't exceed max or go below 0
            character.HP = Math.Max(0, Math.Min(character.HP, character.HPMax));
            character.MP = Math.Max(0, Math.Min(character.MP, character.MPMax));

            // Update character in database
            await _characterRepository.UpdateCharacterAsync(character.Id!, character);
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
