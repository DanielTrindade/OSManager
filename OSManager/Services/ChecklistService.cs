using Microsoft.EntityFrameworkCore;
using OSManager.Data;
using OSManager.Models;

namespace OSManager.Services
{
    public class ChecklistService
    {
        private readonly AppDbContext _context;

        public ChecklistService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChecklistItem>> GetTemplatesAsync()
        {
            return await _context.ChecklistItems
                .Where(c => c.IsTemplate)
                .OrderBy(c => c.Category)
                .ThenBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<List<ChecklistItem>> GetTemplatesByCategoryAsync(string category)
        {
            return await _context.ChecklistItems
                .Where(c => c.IsTemplate && c.Category == category)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ChecklistItem?> GetTemplateByIdAsync(int id)
        {
            return await _context.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.IsTemplate);
        }

        public async Task<ChecklistItem> CreateTemplateAsync(string description, string category, int displayOrder)
        {
            var template = new ChecklistItem
            {
                Description = description,
                Category = category,
                DisplayOrder = displayOrder,
                IsTemplate = true,
                OrderId = null
            };

            _context.ChecklistItems.Add(template);
            await _context.SaveChangesAsync();

            return template;
        }

        public async Task<bool> UpdateTemplateAsync(int id, string description, string category, int displayOrder)
        {
            var template = await _context.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.IsTemplate);

            if (template == null)
                return false;

            template.Description = description;
            template.Category = category;
            template.DisplayOrder = displayOrder;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteTemplateAsync(int id)
        {
            var template = await _context.ChecklistItems
                .FirstOrDefaultAsync(c => c.Id == id && c.IsTemplate);

            if (template == null)
                return false;

            _context.ChecklistItems.Remove(template);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.ChecklistItems
                .Where(c => c.IsTemplate)
                .Select(c => c.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}