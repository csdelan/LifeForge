using LifeForge.DataAccess.Models;
using LifeForge.Domain;

namespace LifeForge.Api.Models
{
    public class ActionDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public List<string> BuffIds { get; set; } = new List<string>();
        public ActionCategory Category { get; set; }
        public int CooldownHours { get; set; } = 0;
    }

    public class PerformActionDto
    {
        public string CharacterId { get; set; } = string.Empty;
        public string ActionId { get; set; } = string.Empty;
    }

    public class ActionResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> ActivatedBuffs { get; set; } = new List<string>();
    }
}
