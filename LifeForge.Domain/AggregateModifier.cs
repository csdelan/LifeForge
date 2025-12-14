namespace LifeForge.Domain
{
    /// <summary>
    /// Represents the aggregated modifiers from all active buffs on a character.
    /// This class is mutable to allow for efficient updates during midnight processing.
    /// </summary>
    public class AggregateModifier : IModifier
    {
        public int HPModifier { get; set; }
        public int HPMaxModifier { get; set; }
        public int HPPercentModifier { get; set; }
        public int HPMaxPercentModifier { get; set; }
        public int MPModifier { get; set; }
        public int MPMaxModifier { get; set; }
        public int MPPercentModifier { get; set; }
        public int MPMaxPercentModifier { get; set; }
        public int XpGainsPercentModifier { get; set; }

        public AggregateModifier()
        {
            // Initialize all to zero
        }

        public void ApplyModifier(IModifier other)
        {
            HPModifier += other.HPModifier;
            HPMaxModifier += other.HPMaxModifier;
            HPPercentModifier += other.HPPercentModifier;
            HPMaxPercentModifier += other.HPMaxPercentModifier;
            MPModifier += other.MPModifier;
            MPMaxModifier += other.MPMaxModifier;
            MPPercentModifier += other.MPPercentModifier;
            MPMaxPercentModifier += other.MPMaxPercentModifier;
            XpGainsPercentModifier += other.XpGainsPercentModifier;
        }

        public void RemoveModifier(IModifier other)
        {
            HPModifier -= other.HPModifier;
            HPMaxModifier -= other.HPMaxModifier;
            HPPercentModifier -= other.HPPercentModifier;
            HPMaxPercentModifier -= other.HPMaxPercentModifier;
            MPModifier -= other.MPModifier;
            MPMaxModifier -= other.MPMaxModifier;
            MPPercentModifier -= other.MPPercentModifier;
            MPMaxPercentModifier -= other.MPMaxPercentModifier;
            XpGainsPercentModifier -= other.XpGainsPercentModifier;
        }

        public IModifier Clone()
        {
            return new AggregateModifier
            {
                HPModifier = this.HPModifier,
                HPMaxModifier = this.HPMaxModifier,
                HPPercentModifier = this.HPPercentModifier,
                HPMaxPercentModifier = this.HPMaxPercentModifier,
                MPModifier = this.MPModifier,
                MPMaxModifier = this.MPMaxModifier,
                MPPercentModifier = this.MPPercentModifier,
                MPMaxPercentModifier = this.MPMaxPercentModifier,
                XpGainsPercentModifier = this.XpGainsPercentModifier
            };
        }

        /// <summary>
        /// Resets all modifiers to zero
        /// </summary>
        public void Reset()
        {
            HPModifier = 0;
            HPMaxModifier = 0;
            HPPercentModifier = 0;
            HPMaxPercentModifier = 0;
            MPModifier = 0;
            MPMaxModifier = 0;
            MPPercentModifier = 0;
            MPMaxPercentModifier = 0;
            XpGainsPercentModifier = 0;
        }
    }
}
