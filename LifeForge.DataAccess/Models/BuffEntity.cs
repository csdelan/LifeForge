using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Models
{
    /// <summary>
    /// MongoDB entity representation of a Buff
    /// </summary>
    public class BuffEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("imageName")]
        public string? ImageName { get; set; }

        [BsonElement("imageData")]
        public string? ImageData { get; set; }

        [BsonElement("imageContentType")]
        public string? ImageContentType { get; set; }

        [BsonElement("isDebuff")]
        public bool IsDebuff { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("trigger")]
        [BsonRepresentation(BsonType.String)]
        public BuffTrigger Trigger { get; set; }

        [BsonElement("maxStacks")]
        public int MaxStacks { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

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

        [BsonElement("durationDays")]
        public int DurationDays { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonExtraElements]
        public BsonDocument? ExtraElements { get; set; }

        /// <summary>
        /// Convert from domain model to entity
        /// </summary>
        public static BuffEntity FromDomain(Buff buff)
        {
            return new BuffEntity
            {
                ImageName = buff.ImagePath,
                IsDebuff = buff.IsDebuff,
                Name = buff.Name,
                Trigger = buff.Trigger,
                MaxStacks = buff.MaxStacks,
                Description = buff.Description,
                HPModifier = buff.HPModifier,
                HPMaxModifier = buff.HPMaxModifier,
                HPPercentModifier = buff.HPPercentModifier,
                HPMaxPercentModifier = buff.HPMaxPercentModifier,
                MPModifier = buff.MPModifier,
                MPMaxModifier = buff.MPMaxModifier,
                MPPercentModifier = buff.MPPercentModifier,
                MPMaxPercentModifier = buff.MPMaxPercentModifier,
                XpGainsPercentModifier = buff.XpGainsPercentModifier,
                DurationDays = buff.DurationDays
            };
        }

        /// <summary>
        /// Convert from entity to domain model
        /// </summary>
        public Buff ToDomain()
        {
            // Handle migration from old TimeSpan duration to new DurationDays
            int durationDays = DurationDays;
            if (durationDays == 0 && ExtraElements != null && ExtraElements.Contains("duration"))
            {
                // Migrate old TimeSpan duration to days
                var durationBson = ExtraElements["duration"];
                if (durationBson.IsString || durationBson.IsBsonDocument)
                {
                    // Try to parse as TimeSpan and convert to days
                    try
                    {
                        if (TimeSpan.TryParse(durationBson.ToString(), out var timeSpan))
                        {
                            durationDays = (int)Math.Ceiling(timeSpan.TotalDays);
                            if (durationDays == 0) durationDays = 1; // Minimum 1 day
                        }
                    }
                    catch
                    {
                        durationDays = 7; // Default to 7 days if parsing fails
                    }
                }
            }

            if (durationDays == 0) durationDays = 7; // Default fallback

            return new Buff
            {
                ImagePath = ImageName,
                IsDebuff = IsDebuff,
                Name = Name,
                Trigger = Trigger,
                MaxStacks = MaxStacks,
                Description = Description,
                HPModifier = HPModifier,
                HPMaxModifier = HPMaxModifier,
                HPPercentModifier = HPPercentModifier,
                HPMaxPercentModifier = HPMaxPercentModifier,
                MPModifier = MPModifier,
                MPMaxModifier = MPMaxModifier,
                MPPercentModifier = MPPercentModifier,
                MPMaxPercentModifier = MPMaxPercentModifier,
                XpGainsPercentModifier = XpGainsPercentModifier,
                DurationDays = durationDays
            };
        }
    }
}
