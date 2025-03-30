namespace OSManager.DTOs
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class UploadImageRequest
    {
        public int OrderId { get; set; }
        public int? ChecklistItemId { get; set; }
    }
}