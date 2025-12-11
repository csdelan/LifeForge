using LifeForge.Domain;

namespace LifeForge.Web.Models
{
    public class QuestDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public string? Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public QuestRepeatability Repeatability { get; set; }
    }
}
