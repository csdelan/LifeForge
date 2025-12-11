using LifeForge.Domain;

namespace LifeForge.Api.Models
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
        public List<RewardDto> Rewards { get; set; } = new List<RewardDto>();
    }

    public class CreateQuestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public string? Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public QuestRepeatability Repeatability { get; set; }
        public List<RewardDto> Rewards { get; set; } = new List<RewardDto>();
    }

    public class UpdateQuestDto
    {
        public string Name { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public string? Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public QuestRepeatability Repeatability { get; set; }
        public List<RewardDto> Rewards { get; set; } = new List<RewardDto>();
    }

    public class RewardDto
    {
        public RewardType Type { get; set; }
        public string RewardClass { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string? Icon { get; set; }
    }
}
