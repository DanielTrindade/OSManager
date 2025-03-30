using OSManager.DTOs;
using OSManager.Models;

namespace OSManager.Extensions
{
    public static class MappingExtensions
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public static OrderResponse ToDto(this Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                Description = order.Description,
                CreatedAt = order.CreatedAt,
                StartedAt = order.StartedAt,
                CompletedAt = order.CompletedAt,
                ApprovedAt = order.ApprovedAt,
                Status = order.Status,
                RejectionReason = order.RejectionReason,
                User = order.User?.ToDto() ?? new UserDto(),
                Approver = order.Approver?.ToDto(),
                ChecklistItems = order.ChecklistItems.Select(c => c.ToDto()).ToList(),
                Images = order.Images.Where(i => i.ChecklistItemId == null).Select(i => i.ToDto()).ToList(),
                AllChecklistItemsCompleted = order.ChecklistItems.Count > 0 && order.ChecklistItems.All(c => c.IsCompleted)
            };
        }

        public static ChecklistItemDto ToDto(this ChecklistItem item)
        {
            return new ChecklistItemDto
            {
                Id = item.Id,
                Description = item.Description,
                IsCompleted = item.IsCompleted,
                Category = item.Category,
                DisplayOrder = item.DisplayOrder,
                Images = item.Images.Select(i => i.ToDto()).ToList()
            };
        }

        public static ChecklistTemplateDto ToTemplateDto(this ChecklistItem item)
        {
            return new ChecklistTemplateDto
            {
                Id = item.Id,
                Description = item.Description,
                Category = item.Category,
                DisplayOrder = item.DisplayOrder
            };
        }

        public static ImageDto ToDto(this Image image)
        {
            return new ImageDto
            {
                Id = image.Id,
                FileName = image.FileName,
                ContentType = image.ContentType,
                FileSize = image.FileSize,
                UploadedAt = image.UploadedAt
            };
        }
    }
}