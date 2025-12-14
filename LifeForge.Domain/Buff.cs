namespace LifeForge.Domain
{
    public enum BuffTrigger
    {
        Action,     // The buff is triggered by a specific action or event
        Manual,     // The buff is applied manually by the user
    }

    public interface IModifier
    {
        /// <summary>
        /// Represents the +/- modifier values for character stats
        /// </summary>
        int HPModifier { get; }
        int HPMaxModifier { get; }
        int HPPercentModifier { get; }
        int HPMaxPercentModifier { get; }
        int MPModifier { get; }
        int MPMaxModifier { get; }
        int MPPercentModifier { get; }
        int MPMaxPercentModifier { get; }

        /// <summary>
        /// This modifier affects the percentage of experience points gained.  For instance,
        /// if the character gains 3 XP from an action, and this modifier is set to 20,
        /// the character would gain an additional 0.6 XP from the buff.
        /// </summary>
        int XpGainsPercentModifier { get; }

        /// <summary>
        /// Adds another modifier's values to this modifier
        /// </summary>
        void ApplyModifier(IModifier other);

        /// <summary>
        /// Subtracts another modifier's values from this modifier
        /// </summary>
        void RemoveModifier(IModifier other);

        /// <summary>
        /// Creates a deep copy of this modifier
        /// </summary>
        IModifier Clone();
    }

    /// <summary>
    /// Represents a buff or debuff that can be applied to a character, caused by various life events or conditions.
    /// </summary>
    public class Buff : IModifier
    {
        public string ImagePath { get; set; }
        public bool IsDebuff { get; set; }
        public string Name { get; set; }
        public BuffTrigger Trigger { get; set; }
        public int MaxStacks { get; set; }
        public string Description { get; set; }

        #region Modifiers

        public int HPModifier { get; set; }
        public int HPMaxModifier { get; set; }
        public int HPPercentModifier { get; set; }
        public int HPMaxPercentModifier { get; set; }
        public int MPModifier { get; set; }
        public int MPMaxModifier { get; set; }
        public int MPPercentModifier { get; set; }
        public int MPMaxPercentModifier { get; set; }
        public int XpGainsPercentModifier { get; set; }

        #endregion

        /// <summary>
        /// Duration of the buff in days
        /// </summary>
        public int DurationDays { get; set; }

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
            return new Buff
            {
                ImagePath = this.ImagePath,
                IsDebuff = this.IsDebuff,
                Name = this.Name,
                Trigger = this.Trigger,
                MaxStacks = this.MaxStacks,
                Description = this.Description,
                HPModifier = this.HPModifier,
                HPMaxModifier = this.HPMaxModifier,
                HPPercentModifier = this.HPPercentModifier,
                HPMaxPercentModifier = this.HPMaxPercentModifier,
                MPModifier = this.MPModifier,
                MPMaxModifier = this.MPMaxModifier,
                MPPercentModifier = this.MPPercentModifier,
                MPMaxPercentModifier = this.MPMaxPercentModifier,
                XpGainsPercentModifier = this.XpGainsPercentModifier,
                DurationDays = this.DurationDays
            };
        }
    }
}
