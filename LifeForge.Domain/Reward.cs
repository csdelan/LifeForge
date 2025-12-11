namespace LifeForge.Domain
{
    public enum CurrencyType
    {
        Gold,
        Karma,
        DesignWorkslot,
    }

    public enum RewardType
    {
        Experience, // XP Points to be applied to a specific character class
        Currency,   // e.g., Gold, Karma, etc
        Item,       // Placeholder for future
        Badge       // Placeholder for future
    }

    /// <summary>
    /// A reward is given to a character upon completion of a quest or achievement.
    /// </summary>
    public class Reward
    {
        public string Icon { get; set; } = string.Empty;
        public RewardType Type { get; set; }
        public string RewardClass { get; set; } = string.Empty;
        public int Amount { get; set; }

        public Reward()
        {
        }

        public Reward(RewardType type, string rewardClass, int amount, string icon = "🪙")
        {
            Type = type;
            RewardClass = rewardClass;
            Amount = amount;
            Icon = icon;
        }
    }
}
