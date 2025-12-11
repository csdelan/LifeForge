namespace LifeForge.Domain
{
    /// <summary>
    /// Represents an active instance of a buff, including its associated buff definition and the time at which it was
    /// applied.
    /// </summary>
    /// <remarks>A <see cref="BuffInstance"/> tracks the start time and duration of a buff effect. The
    /// instance provides access to the buff's metadata and calculates its end time based on the buff's
    /// duration.</remarks>
    public class BuffInstance : ILifeEvent
    {
        public Buff Buff { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime => StartTime.AddDays(Buff.DurationDays);
        public string Name => Buff.Name;
        public BuffInstance(Buff buff)
        {
            Buff = buff;
            StartTime = DateTime.Now;
        }

        public string DescribeStart()
        {
            return $"Buff '{Name}' applied.";
        }

        public string DescribeEnd()
        {
            return $"Buff '{Name}' expired.";
        }
    }
}
