namespace LifeForge.Domain
{
    /// <summary>
    /// LifeForge typically revolves around a single character. This class
    /// represents that character, including their stats, currencies, classes, etc.
    /// It is assumed that there is only 1 character, representing the USER.
    /// </summary>
    public class Character
    {
        public string Name { get; }

        /// <summary>
        /// Current hit points (base/un-buffed value)
        /// </summary>
        public decimal HP { get; set; }

        /// <summary>
        /// Maximum hit points (base/un-buffed value)
        /// </summary>
        public decimal HPMax { get; set; }

        /// <summary>
        /// Current mana points (base/un-buffed value)
        /// </summary>
        public decimal MP { get; set; }

        /// <summary>
        /// Maximum mana points (base/un-buffed value)
        /// </summary>
        public decimal MPMax { get; set; }

        public int Strength { get; set; }
        public int Discipline { get; set; }
        public int Focus { get; set; }
        public Dictionary<CurrencyType, int> Currencies { get; set; } = new();
        public Dictionary<string, CharacterClassSnapshot> ClassProfiles { get; set; } = new();
        public List<BuffInstance> ActiveBuffs { get; set; } = new();

        /// <summary>
        /// Aggregate of all active buff modifiers. Updated by midnight job and manual buff operations.
        /// </summary>
        public AggregateModifier ActiveBuffModifiers { get; set; } = new();

        public Character(string name)
        {
            Name = name;
        }

        #region Effective Stats (Display Values)

        /// <summary>
        /// Calculates effective HP with buff modifiers applied
        /// </summary>
        public decimal EffectiveHP
        {
            get
            {
                decimal effective = HP + ActiveBuffModifiers.HPModifier;
                effective += effective * (ActiveBuffModifiers.HPPercentModifier / 100.0m);
                return Math.Max(0, Math.Min(effective, EffectiveHPMax));
            }
        }

        /// <summary>
        /// Calculates effective HP Max with buff modifiers applied
        /// </summary>
        public decimal EffectiveHPMax
        {
            get
            {
                decimal effective = HPMax + ActiveBuffModifiers.HPMaxModifier;
                effective += effective * (ActiveBuffModifiers.HPMaxPercentModifier / 100.0m);
                return Math.Max(0, effective);
            }
        }

        /// <summary>
        /// Calculates effective MP with buff modifiers applied
        /// </summary>
        public decimal EffectiveMP
        {
            get
            {
                decimal effective = MP + ActiveBuffModifiers.MPModifier;
                effective += effective * (ActiveBuffModifiers.MPPercentModifier / 100.0m);
                return Math.Max(0, Math.Min(effective, EffectiveMPMax));
            }
        }

        /// <summary>
        /// Calculates effective MP Max with buff modifiers applied
        /// </summary>
        public decimal EffectiveMPMax
        {
            get
            {
                decimal effective = MPMax + ActiveBuffModifiers.MPMaxModifier;
                effective += effective * (ActiveBuffModifiers.MPMaxPercentModifier / 100.0m);
                return Math.Max(0, effective);
            }
        }

        #endregion

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
        /// Add experience to a specific character class, applying XP gain modifiers from buffs
        /// </summary>
        public void AddExperience(string className, int amount)
        {
            if (amount < 0)
                throw new ArgumentException("Amount must be non-negative", nameof(amount));

            // Apply XP gains modifier from active buffs
            int xpBonus = (int)(amount * (ActiveBuffModifiers.XpGainsPercentModifier / 100.0m));
            int totalAmount = amount + xpBonus;

            if (ClassProfiles.TryGetValue(className, out var profile))
            {
                // CharacterClassSnapshot is immutable, so we need to create a new one with updated XP
                int newXp = profile.CurrentXp + totalAmount;
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
                ClassProfiles[className] = new CharacterClassSnapshot(newClass, 1, totalAmount);
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
