namespace LifeForge.Domain
{
    /// <summary>
    /// Represents a common action that can be performed, which may activate one or more buffs
    /// </summary>
    public class Action
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public List<string> BuffIds { get; set; } = new List<string>();
        public ActionCategory Category { get; set; }
        public int CooldownHours { get; set; } = 0;

        public Action()
        {
        }

        public Action(string name, string description, string icon, ActionCategory category)
        {
            Name = name;
            Description = description;
            Icon = icon;
            Category = category;
        }
    }

    public enum ActionCategory
    {
        Social,
        Health,
        Recreation,
        Work,
        Other
    }
}
