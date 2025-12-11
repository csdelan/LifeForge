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
        public string ImagePath { get; set; }
        public RewardType Type { get; }
        public string RewardClass { get; set; }
        public int Amount { get; }

        public Reward()
        {
        }
    }
}
