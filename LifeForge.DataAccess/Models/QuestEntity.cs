using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Models
{
    /// <summary>
    /// MongoDB entity representation of a Quest
    /// </summary>
    public class QuestEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("imageName")]
        public string? ImageName { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("difficulty")]
        [BsonRepresentation(BsonType.String)]
        public DifficultyLevel Difficulty { get; set; }

        [BsonElement("repeatability")]
        [BsonRepresentation(BsonType.String)]
        public QuestRepeatability Repeatability { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Convert from domain model to entity
        /// </summary>
        public static QuestEntity FromDomain(Quest quest)
        {
            return new QuestEntity
            {
                Name = quest.Name,
                ImageName = quest.ImageName,
                Description = quest.Description,
                Difficulty = quest.Difficulty,
                Repeatability = quest.Repeatability
            };
        }

        /// <summary>
        /// Convert from entity to domain model
        /// </summary>
        public Quest ToDomain()
        {
            return new Quest(Name)
            {
                ImageName = ImageName,
                Description = Description,
                Difficulty = Difficulty,
                Repeatability = Repeatability
            };
        }
    }
}
