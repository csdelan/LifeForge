namespace LifeForge.Domain
{
    /// <summary>
    /// Represents a generic life event.  This could be an action taken to run a quest,
    /// or it could be a buff/debug acquired from a specific condition, or similar.
    /// </summary>
    public interface ILifeEvent
    {
        string Name { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public string DescribeStart();
        public string DescribeEnd();
    }
}
