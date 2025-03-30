namespace OSManager.Models
{
    public class ChecklistItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public bool IsTemplate { get; set; } = false;
        public string Category { get; set; } = "General";
        public int DisplayOrder { get; set; } = 0;
        public List<Image> Images { get; set; } = new();
    }
}