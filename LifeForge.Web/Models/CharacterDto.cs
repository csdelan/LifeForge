using LifeForge.Domain;

namespace LifeForge.Web.Models
{
    public class CharacterDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal HP { get; set; }
        public decimal HPMax { get; set; }
        public decimal MP { get; set; }
        public decimal MPMax { get; set; }
        public int Strength { get; set; }
        public int Discipline { get; set; }
        public int Focus { get; set; }
        public Dictionary<CurrencyType, int> Currencies { get; set; } = new();
        public Dictionary<string, CharacterClassDto> ClassProfiles { get; set; } = new();
        public AggregateModifierDto ActiveBuffModifiers { get; set; } = new();
        
        // Computed effective stats (with buffs applied)
        public decimal EffectiveHP { get; set; }
        public decimal EffectiveHPMax { get; set; }
        public decimal EffectiveMP { get; set; }
        public decimal EffectiveMPMax { get; set; }
    }

    public class CharacterClassDto
    {
        public string ClassName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int CurrentXp { get; set; }
        public int XpToNextLevel { get; set; }
    }

    public class AggregateModifierDto
    {
        public int HPModifier { get; set; }
        public int HPMaxModifier { get; set; }
        public int HPPercentModifier { get; set; }
        public int HPMaxPercentModifier { get; set; }
        public int MPModifier { get; set; }
        public int MPMaxModifier { get; set; }
        public int MPPercentModifier { get; set; }
        public int MPMaxPercentModifier { get; set; }
        public int XpGainsPercentModifier { get; set; }
    }

    public class RewardApplicationResultDto
    {
        public bool Success { get; set; }
        public List<string> AppliedRewards { get; set; } = new();
        public Dictionary<string, int> CurrenciesGained { get; set; } = new();
        public Dictionary<string, int> ExperienceGained { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
