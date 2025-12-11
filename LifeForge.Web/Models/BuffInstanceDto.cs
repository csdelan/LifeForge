using LifeForge.Domain;

namespace LifeForge.Web.Models
{
    public class BuffInstanceDto
    {
        public string? Id { get; set; }
        public string BuffId { get; set; } = string.Empty;
        public string CharacterId { get; set; } = string.Empty;
        public string BuffName { get; set; } = string.Empty;
        public bool IsDebuff { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Stacks { get; set; }
        public bool IsActive { get; set; }
        public int HPModifier { get; set; }
        public int HPMaxModifier { get; set; }
        public int HPPercentModifier { get; set; }
        public int HPMaxPercentModifier { get; set; }
        public int MPModifier { get; set; }
        public int MPMaxModifier { get; set; }
        public int MPPercentModifier { get; set; }
        public int MPMaxPercentModifier { get; set; }
        public int XpGainsPercentModifier { get; set; }
        
        // Image data for display
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
    }

    public class BuffInstanceApplicationResultDto
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? BuffInstanceId { get; set; }
        public Dictionary<string, int> ModifiersApplied { get; set; } = new();
    }
}
