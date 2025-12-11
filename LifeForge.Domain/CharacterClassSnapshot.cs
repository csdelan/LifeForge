namespace LifeForge.Domain
{
    /// <summary>
    /// Represents the current status of a specific character class, and corresponding
    /// skill tree TODO: Skill Tree implementation not done yet
    /// </summary>
    public class CharacterClassSnapshot
    {
        public CharacterClass Class { get; }
        public int Level { get; }
        public int CurrentXp { get; }
        public int XpToNextLevel { get; }
        public CharacterClassSnapshot(CharacterClass characterClass, int level, int currentXp)
        {
            Class = characterClass;
            Level = level;
            CurrentXp = currentXp;
            XpToNextLevel = CalculateXpToNextLevel();
        }
        private int CalculateXpToNextLevel()
        {
            double xpNeeded = Class.BaseXp * Math.Pow(Class.XpMultiplier, Level - 1);
            return (int)xpNeeded;
        }
    }
}
