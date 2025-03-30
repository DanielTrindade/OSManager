namespace OSManager.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string Status { get; set; } = "Created"; // Created, InProgress, Completed, Approved, Rejected
        public string? RejectionReason { get; set; }
        public int UserId { get; set; }
        public int? ApproverId { get; set; }
        public User? User { get; set; }
        public User? Approver { get; set; }
        public List<ChecklistItem> ChecklistItems { get; set; } = new();
        public List<Image> Images { get; set; } = new();
    }
}