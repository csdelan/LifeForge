namespace LifeForge.Domain
{
    /// <summary>
    /// Represents a single run of a quest by a character.  This is used to track
    /// the progress, status, rewards of the quest.
    /// </summary>
    public class QuestRun : ILifeEvent
    {
        public Quest Quest { get; }
        public string Name => Quest.Name;
        public QuestStatus Status { get; private set; }

        public DateTime StartTime => throw new NotImplementedException();

        public DateTime EndTime => throw new NotImplementedException();

        public Dictionary<RewardType, Reward> Rewards => throw new NotImplementedException();

        public QuestRun(Quest quest)
        {
            Quest = quest;
            Status = QuestStatus.NotStarted;
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
