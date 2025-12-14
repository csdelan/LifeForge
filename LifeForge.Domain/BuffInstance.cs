namespace LifeForge.Domain
{
    /// <summary>
    /// Represents the lifecycle status of a buff instance
    /// </summary>
    public enum BuffInstanceStatus
    {
        /// <summary>
        /// Buff has been created but not yet processed by the midnight job
        /// </summary>
        Pending,

        /// <summary>
        /// Buff is currently active and contributing to character modifiers
        /// </summary>
        Active,

        /// <summary>
        /// Buff has expired and is pending removal by the midnight job
        /// </summary>
        Expired
    }

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
        public BuffInstanceStatus Status { get; set; } = BuffInstanceStatus.Pending;

        public BuffInstance(Buff buff)
        {
            Buff = buff;
            StartTime = DateTime.UtcNow;
        }

        public string DescribeStart()
        {
            return $"Buff '{Name}' applied.";
        }

        public string DescribeEnd()
        {
            return $"Buff '{Name}' expired.";
        }

        /// <summary>
        /// Checks if this buff instance has expired based on current time
        /// </summary>
        public bool IsExpired()
        {
            return DateTime.UtcNow >= EndTime;
        }
    }
}
