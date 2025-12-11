using LifeForge.Domain;

namespace LifeForge.Web.Models
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

    public class RewardDto
    {
        public RewardType Type { get; set; }
        public string RewardClass { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string? Icon { get; set; }
    }

    public class StartQuestRunDto
    {
        public string QuestId { get; set; } = string.Empty;
    }
}
