namespace LifeForge.Application.Models
{
    public class BuffInstanceApplicationResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? BuffInstanceId { get; set; }
        public Dictionary<string, int> ModifiersApplied { get; set; } = new();
    }
}
