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
    }
}
