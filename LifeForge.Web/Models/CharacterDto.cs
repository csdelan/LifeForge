namespace LifeForge.Web.Models
{
    public class CharacterDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int MP { get; set; }
        public int MPMax { get; set; }
        public int Strength { get; set; }
        public int Discipline { get; set; }
        public int Focus { get; set; }
        public Dictionary<string, int> Currencies { get; set; } = new();
        public Dictionary<string, CharacterClassDto> ClassProfiles { get; set; } = new();
    }

    public class CharacterClassDto
    {
        public string ClassName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int CurrentXp { get; set; }
        public int XpToNextLevel { get; set; }
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
