using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using LifeForge.Domain;

namespace LifeForge.DataAccess.Models
{
    /// <summary>
    /// MongoDB entity representation of an Action
    /// </summary>
    public class ActionEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("icon")]
        public string Icon { get; set; } = string.Empty;

        [BsonElement("imageName")]
        public string? ImageName { get; set; }

        [BsonElement("imageData")]
        public string? ImageData { get; set; }

        [BsonElement("imageContentType")]
        public string? ImageContentType { get; set; }

        [BsonElement("buffIds")]
        public List<string> BuffIds { get; set; } = new List<string>();

        [BsonElement("category")]
        [BsonRepresentation(BsonType.String)]
        public ActionCategory Category { get; set; }

        [BsonElement("cooldownHours")]
        public int CooldownHours { get; set; } = 0;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public static ActionEntity FromDomain(Domain.Action action)
        {
            return new ActionEntity
            {
                Name = action.Name,
                Description = action.Description,
                Icon = action.Icon,
                ImageName = action.ImageName,
                ImageData = action.ImageData,
                ImageContentType = action.ImageContentType,
                BuffIds = action.BuffIds,
                Category = action.Category,
                CooldownHours = action.CooldownHours
            };
        }

        public Domain.Action ToDomain()
        {
            return new Domain.Action
            {
                Name = Name,
                Description = Description,
                Icon = Icon,
                ImageName = ImageName,
                ImageData = ImageData,
                ImageContentType = ImageContentType,
                BuffIds = BuffIds,
                Category = Category,
                CooldownHours = CooldownHours
            };
        }
    }
}
