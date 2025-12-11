namespace LifeForge.Domain
{
    /// <summary>
    /// Represents a character class for real life, such as Trader, Developer, Designer, etc.
    /// </summary>
    public class CharacterClass
    {
        public string Name { get; }
        public uint BaseXp { get; set; } = 100;
        public double XpMultiplier { get; set; } = 1.1;

        public CharacterClass(string name)
        {
            Name = name;
        }
    }
}
