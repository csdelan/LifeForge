using MongoDB.Bson.Serialization.Attributes;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Models
{
    /// <summary>
    /// MongoDB entity representation of an AggregateModifier
    /// </summary>
    public class AggregateModifierEntity
    {
        [BsonElement("hpModifier")]
        public int HPModifier { get; set; }

        [BsonElement("hpMaxModifier")]
        public int HPMaxModifier { get; set; }

        [BsonElement("hpPercentModifier")]
        public int HPPercentModifier { get; set; }

        [BsonElement("hpMaxPercentModifier")]
        public int HPMaxPercentModifier { get; set; }

        [BsonElement("mpModifier")]
        public int MPModifier { get; set; }

        [BsonElement("mpMaxModifier")]
        public int MPMaxModifier { get; set; }

        [BsonElement("mpPercentModifier")]
        public int MPPercentModifier { get; set; }

        [BsonElement("mpMaxPercentModifier")]
        public int MPMaxPercentModifier { get; set; }

        [BsonElement("xpGainsPercentModifier")]
        public int XpGainsPercentModifier { get; set; }

        /// <summary>
        /// Convert from domain model to entity
        /// </summary>
        public static AggregateModifierEntity FromDomain(AggregateModifier modifier)
        {
            return new AggregateModifierEntity
            {
                HPModifier = modifier.HPModifier,
                HPMaxModifier = modifier.HPMaxModifier,
                HPPercentModifier = modifier.HPPercentModifier,
                HPMaxPercentModifier = modifier.HPMaxPercentModifier,
                MPModifier = modifier.MPModifier,
                MPMaxModifier = modifier.MPMaxModifier,
                MPPercentModifier = modifier.MPPercentModifier,
                MPMaxPercentModifier = modifier.MPMaxPercentModifier,
                XpGainsPercentModifier = modifier.XpGainsPercentModifier
            };
        }

        /// <summary>
        /// Convert from entity to domain model
        /// </summary>
        public AggregateModifier ToDomain()
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
    }
}
