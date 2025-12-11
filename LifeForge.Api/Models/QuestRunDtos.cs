using LifeForge.Domain;

namespace LifeForge.Api.Models
{
    public class QuestRunDto
    {
        public string? Id { get; set; }
        public string QuestId { get; set; } = string.Empty;
        public string QuestName { get; set; } = string.Empty;
        public QuestStatus Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<RewardDto> Rewards { get; set; } = new List<RewardDto>();
    }

    public class StartQuestRunDto
    {
        public string QuestId { get; set; } = string.Empty;
    }

    public class CompleteQuestRunDto
    {
        public List<RewardDto> Rewards { get; set; } = new List<RewardDto>();
    }
}
