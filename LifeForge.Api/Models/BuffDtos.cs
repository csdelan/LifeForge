using LifeForge.Domain;

namespace LifeForge.Api.Models
{
    public class BuffDto
    {
        public string? Id { get; set; }
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public bool IsDebuff { get; set; }
        public string Name { get; set; } = string.Empty;
        public BuffTrigger Trigger { get; set; }
        public int MaxStacks { get; set; }
        public string? Description { get; set; }
        public int HPModifier { get; set; }
        public int HPMaxModifier { get; set; }
        public int HPPercentModifier { get; set; }
        public int HPMaxPercentModifier { get; set; }
        public int MPModifier { get; set; }
        public int MPMaxModifier { get; set; }
        public int MPPercentModifier { get; set; }
        public int MPMaxPercentModifier { get; set; }
        public int XpGainsPercentModifier { get; set; }
        public int DurationDays { get; set; }
    }

    public class CreateBuffDto
    {
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public bool IsDebuff { get; set; }
        public string Name { get; set; } = string.Empty;
        public BuffTrigger Trigger { get; set; }
        public int MaxStacks { get; set; }
        public string? Description { get; set; }
        public int HPModifier { get; set; }
        public int HPMaxModifier { get; set; }
        public int HPPercentModifier { get; set; }
        public int HPMaxPercentModifier { get; set; }
        public int MPModifier { get; set; }
        public int MPMaxModifier { get; set; }
        public int MPPercentModifier { get; set; }
        public int MPMaxPercentModifier { get; set; }
        public int XpGainsPercentModifier { get; set; }
        public int DurationDays { get; set; }
    }

    public class UpdateBuffDto
    {
        public string? ImageName { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        public bool IsDebuff { get; set; }
        public string Name { get; set; } = string.Empty;
        public BuffTrigger Trigger { get; set; }
        public int MaxStacks { get; set; }
        public string? Description { get; set; }
        public int HPModifier { get; set; }
        public int HPMaxModifier { get; set; }
        public int HPPercentModifier { get; set; }
        public int HPMaxPercentModifier { get; set; }
        public int MPModifier { get; set; }
        public int MPMaxModifier { get; set; }
        public int MPPercentModifier { get; set; }
        public int MPMaxPercentModifier { get; set; }
        public int XpGainsPercentModifier { get; set; }
        public int DurationDays { get; set; }
    }
}
