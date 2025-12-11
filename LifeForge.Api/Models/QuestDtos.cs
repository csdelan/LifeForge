using LifeForge.Domain;

namespace LifeForge.Api.Models
{
    public class QuestDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public QuestRepeatability Repeatability { get; set; }
    }

    public class CreateQuestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public QuestRepeatability Repeatability { get; set; }
    }

    public class UpdateQuestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public QuestRepeatability Repeatability { get; set; }
    }
}
