namespace OSManager.DTOs
{
    public class ChecklistItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public List<ImageDto> Images { get; set; } = new();
    }

    public class UpdateChecklistItemRequest
    {
        public int Id { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class ChecklistTemplateDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }

    public class CreateChecklistTemplateRequest
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public int DisplayOrder { get; set; } = 0;
    }

    public class UpdateChecklistTemplateRequest
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}