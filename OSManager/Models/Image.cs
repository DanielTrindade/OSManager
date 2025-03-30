namespace OSManager.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int? ChecklistItemId { get; set; }
        public ChecklistItem? ChecklistItem { get; set; }
    }
}