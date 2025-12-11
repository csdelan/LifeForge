namespace LifeForge.Domain
{

    public enum QuestRepeatability
    {
        OneTime,
        Unlimited,
        Daily,
        Weekly,
        Monthly
    }

    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Failed
    }

    public enum DifficultyLevel
    {
        Trivial,
        Easy,
        Medium,
        Hard,
        CrazyHard
    }

    /// <summary>
    /// A quest is a task or set of tasks that a character can undertake to earn rewards.
    /// </summary>
    public class Quest
    {
        public string Name { get; }
        public string ImageName { get; set; }
        public string Description { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public QuestRepeatability Repeatability { get; set; }

        public Quest(string Name)
        {
            this.Name = Name;
            Repeatability = QuestRepeatability.OneTime;
        }
    }
}
