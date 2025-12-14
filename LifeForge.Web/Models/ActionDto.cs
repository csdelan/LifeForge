using LifeForge.Domain;

namespace LifeForge.Web.Models
{
    public class ActionDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public List<string> BuffIds { get; set; } = new List<string>();
        public ActionCategory Category { get; set; }
        public int CooldownHours { get; set; } = 0;
    }

    public class ActionResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> ActivatedBuffs { get; set; } = new List<string>();
    }
}
