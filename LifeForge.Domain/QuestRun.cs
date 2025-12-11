namespace LifeForge.Domain
{
    /// <summary>
    /// Represents a single run of a quest by a character.  This is used to track
    /// the progress, status, rewards of the quest.
    /// </summary>
    public class QuestRun : ILifeEvent
    {
        private DateTime _startTime;
        private DateTime? _endTime;
        private Dictionary<RewardType, Reward> _rewards;

        public Quest Quest { get; }
        public string Name => Quest.Name;
        public QuestStatus Status { get; private set; }

        public DateTime StartTime => _startTime;

        public DateTime EndTime => _endTime ?? DateTime.MaxValue;

        public Dictionary<RewardType, Reward> Rewards => _rewards;

        public QuestRun(Quest quest)
        {
            Quest = quest;
            Status = QuestStatus.NotStarted;
            _startTime = DateTime.MinValue;
            _endTime = null;
            _rewards = new Dictionary<RewardType, Reward>();
        }

        public void Start()
        {
            if (Status != QuestStatus.NotStarted)
            {
                throw new InvalidOperationException($"Cannot start a quest that is already {Status}");
            }

            _startTime = DateTime.UtcNow;
            Status = QuestStatus.InProgress;
        }

        public void Complete(Dictionary<RewardType, Reward> rewards)
        {
            if (Status != QuestStatus.InProgress)
            {
                throw new InvalidOperationException($"Cannot complete a quest that is {Status}");
            }

            _endTime = DateTime.UtcNow;
            Status = QuestStatus.Completed;
            _rewards = rewards;
        }

        public void Fail()
        {
            if (Status != QuestStatus.InProgress)
            {
                throw new InvalidOperationException($"Cannot fail a quest that is {Status}");
            }

            _endTime = DateTime.UtcNow;
            Status = QuestStatus.Failed;
        }

        public string DescribeStart()
        {
            return $"Quest '{Name}' started.";
        }

        public string DescribeEnd()
        {
            return $"Quest '{Name}' ended.";
        }
    }
}
