using LifeForge.Domain;

namespace LifeForge.Application.Models
{
    /// <summary>
    /// Result of applying rewards to a character
    /// </summary>
    public class RewardApplicationResult
    {
        public bool Success { get; set; }
        public List<string> AppliedRewards { get; set; } = new();
        public Dictionary<string, int> CurrenciesGained { get; set; } = new();
        public Dictionary<string, int> ExperienceGained { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}
