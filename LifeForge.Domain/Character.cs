namespace LifeForge.Domain
{
    /// <summary>
    /// LifeForge typically revolves around a single characte.  This class
    /// represents that character, including their stats, currencies, classes, etc.
    /// It is assumed that there is only 1 character, representing the USER.
    /// </summary>
    public class Character
    {
        public string Name { get; }

        /// <summary>
        /// current hit points
        /// </summary>
        public int HP { get; set; }

        /// <summary>
        /// Maximum hit points
        /// </summary>
        public int HPMax { get; set; }

        /// <summary>
        /// Current mana points
        /// </summary>
        public int MP { get; set; }

        /// <summary>
        /// Maximum mana points
        /// </summary>
        public int MPMax { get; set; }
        public int Strength { get; set; }
        public int Discipline { get; set; }
        public int Focus { get; set; }
        public Dictionary<CurrencyType, int> Currencies { get; set; } = new();
        public Dictionary<string, CharacterClassSnapshot> ClassProfiles { get; set; } = new();
        public List<BuffInstance> ActiveBuffs { get; set; } = new();

        public Character(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Add currency to the character's wallet
        /// </summary>
        public void AddCurrency(CurrencyType type, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be non-negative", nameof(amount));

            if (Currencies.ContainsKey(type))
                Currencies[type] += amount;
            else
                Currencies[type] = amount;
        }

        /// <summary>
        /// Add experience to a specific character class
        /// </summary>
        public void AddExperience(string className, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be non-negative", nameof(amount));

            if (ClassProfiles.TryGetValue(className, out var profile))
            {
                // CharacterClassSnapshot is immutable, so we need to create a new one with updated XP
                int newXp = profile.CurrentXp + amount;
                int newLevel = profile.Level;

                // Check if we've leveled up
                while (newXp >= profile.XpToNextLevel)
                {
                    newXp -= profile.XpToNextLevel;
                    newLevel++;
                }

                // Replace with new snapshot
                ClassProfiles[className] = new CharacterClassSnapshot(profile.Class, newLevel, newXp);
            }
            else
            {
                // Class doesn't exist yet, create it
                var newClass = new CharacterClass(className);
                ClassProfiles[className] = new CharacterClassSnapshot(newClass, 1, amount);
            }
        }

        /// <summary>
        /// Apply a reward to the character
        /// </summary>
        public void ApplyReward(Reward reward)
        {
            switch (reward.Type)
            {
                case RewardType.Currency:
                    if (Enum.TryParse<CurrencyType>(reward.RewardClass, out var currencyType))
                        AddCurrency(currencyType, reward.Amount);
                    break;
                case RewardType.Experience:
                    AddExperience(reward.RewardClass, reward.Amount);
                    break;
                case RewardType.Item:
                    // TODO: Implement item rewards when inventory system is added
                    break;
                case RewardType.Badge:
                    // TODO: Implement badge rewards when achievement system is added
                    break;
            }
        }
    }
}
