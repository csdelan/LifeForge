using LifeForge.Domain;

namespace LifeForge.Web.Models
{
    /// <summary>
    /// Defines available reward types with their metadata
    /// </summary>
    public class RewardDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public RewardType Type { get; set; }
        public string RewardClass { get; set; } = string.Empty;

        public static List<RewardDefinition> GetAvailableRewards()
        {
            return new List<RewardDefinition>
            {
                new RewardDefinition { Name = "Gold", Icon = "\U0001FA99", Type = RewardType.Currency, RewardClass = "Gold" },
                new RewardDefinition { Name = "Karma", Icon = "\u262F\uFE0F", Type = RewardType.Currency, RewardClass = "Karma" },
                new RewardDefinition { Name = "Design Workslot", Icon = "\U0001F3A8", Type = RewardType.Currency, RewardClass = "DesignWorkslot" }
            };
        }

        public static RewardDefinition? GetByRewardClass(string rewardClass)
        {
            return GetAvailableRewards().FirstOrDefault(r => r.RewardClass == rewardClass);
        }

        public static RewardDefinition? GetByName(string name)
        {
            return GetAvailableRewards().FirstOrDefault(r => r.Name == name);
        }

        public static string GetIconForRewardClass(string rewardClass)
        {
            var reward = GetAvailableRewards().FirstOrDefault(r => r.RewardClass == rewardClass);
            return reward?.Icon ?? "??";
        }
    }
}
