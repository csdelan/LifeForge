using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Models
{
    /// <summary>
    /// MongoDB entity representation of a QuestRun
    /// </summary>
    public class QuestRunEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("questId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string QuestId { get; set; } = string.Empty;

        [BsonElement("questName")]
        public string QuestName { get; set; } = string.Empty;

        [BsonElement("status")]
        [BsonRepresentation(BsonType.String)]
        public QuestStatus Status { get; set; }

        [BsonElement("startTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("endTime")]
        public DateTime? EndTime { get; set; }

        [BsonElement("rewards")]
        public List<RewardEntity> Rewards { get; set; } = new List<RewardEntity>();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// MongoDB representation of a Reward
    /// </summary>
    public class RewardEntity
    {
        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public RewardType Type { get; set; }

        [BsonElement("rewardClass")]
        public string RewardClass { get; set; } = string.Empty;

        [BsonElement("amount")]
        public int Amount { get; set; }

        [BsonElement("icon")]
        public string? Icon { get; set; }

        [BsonElement("imagePath")]
        public string? ImagePath { get; set; }
    }
}
