using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Models
{
    /// <summary>
    /// MongoDB entity representation of a BuffInstance (active buff)
    /// </summary>
    public class BuffInstanceEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("buffId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BuffId { get; set; } = string.Empty;

        [BsonElement("characterId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CharacterId { get; set; } = string.Empty;

        [BsonElement("buffName")]
        public string BuffName { get; set; } = string.Empty;

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("isDebuff")]
        public bool IsDebuff { get; set; }

        [BsonElement("startTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("endTime")]
        public DateTime EndTime { get; set; }

        [BsonElement("stacks")]
        public int Stacks { get; set; } = 1;

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        // Store buff modifiers for when we need to unapply them
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

        // Image data for display
        [BsonElement("imageData")]
        public string? ImageData { get; set; }

        [BsonElement("imageContentType")]
        public string? ImageContentType { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
