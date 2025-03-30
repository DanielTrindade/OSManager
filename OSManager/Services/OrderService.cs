using Microsoft.EntityFrameworkCore;
using OSManager.Data;
using OSManager.DTOs;
using OSManager.Models;

namespace OSSystem.Services
{
    public class OrderStatistics
    {
        public int TotalOrders { get; set; }
        public int CreatedOrders { get; set; }
        public int InProgressOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int ApprovedOrders { get; set; }
        public int RejectedOrders { get; set; }
    }

    public class OrderService(AppDbContext context)
    {
        public async Task<List<Order>> GetOrdersAsync(int userId)
        {
            return await context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.ChecklistItems)
                .Include(o => o.Images)
                .Include(o => o.User)
                .Include(o => o.Approver)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await context.Orders
                .Include(o => o.ChecklistItems)
                .Include(o => o.Images)
                .Include(o => o.User)
                .Include(o => o.Approver)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await context.Orders
                .Include(o => o.ChecklistItems)
                .Include(o => o.Images)
                .Include(o => o.User)
                .Include(o => o.Approver)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateOrderAsync(string description, int userId)
        {
            // Criar a ordem
            var order = new Order
            {
                Description = description,
                UserId = userId
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Obter os templates de checklist
            var templateItems = await context.ChecklistItems
                .Where(c => c.IsTemplate)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            // Criar itens de checklist para a ordem
            foreach (var template in templateItems)
            {
                var item = new ChecklistItem
                {
                    Description = template.Description,
                    Category = template.Category,
                    DisplayOrder = template.DisplayOrder,
                    IsCompleted = false,
                    OrderId = order.Id
                };

                context.ChecklistItems.Add(item);
            }

            await context.SaveChangesAsync();

            // Retornar a ordem com os itens incluídos
            return await GetOrderByIdAsync(order.Id) ?? order;
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string status, string? rejectionReason = null, int? approverId = null)
        {
            var order = await context.Orders
                .Include(o => o.ChecklistItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return false;

            // Validar status permitidos e transições
            var allowedStatuses = new[] { "Created", "InProgress", "Completed", "Approved", "Rejected" };
            if (!allowedStatuses.Contains(status))
                return false;

            // Validar transições de status
            if (status == "Completed")
            {
                // Verificar se todos os itens do checklist estão completos
                if (order.ChecklistItems.Any(c => !c.IsCompleted))
                    return false;
            }

            // Atualizar campos com base no status
            switch (status)
            {
                case "InProgress":
                    order.StartedAt = DateTime.UtcNow;
                    break;
                case "Completed":
                    order.CompletedAt = DateTime.UtcNow;
                    break;
                case "Approved":
                    order.ApprovedAt = DateTime.UtcNow;
                    order.ApproverId = approverId;
                    break;
                case "Rejected":
                    order.RejectionReason = rejectionReason;
                    order.ApproverId = approverId;
                    break;
            }

            order.Status = status;
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateChecklistItemAsync(int id, bool isCompleted)
        {
            var item = await context.ChecklistItems.FindAsync(id);

            if (item == null)
                return false;

            item.IsCompleted = isCompleted;
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Order>> GetFilteredOrdersAsync(int? userId = null, string? status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            IQueryable<Order> query = context.Orders
                .Include(o => o.ChecklistItems)
                .Include(o => o.Images)
                .Include(o => o.User)
                .Include(o => o.Approver);

            // Aplicar filtros
            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status == status);

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt <= toDate.Value.AddDays(1));

            return await query.ToListAsync();
        }

        public async Task<OrderStatistics> GetOrderStatisticsAsync(int? userId = null)
        {
            IQueryable<Order> query = context.Orders;

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            // Agrupar por status e contar
            var statusCounts = await query
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            // Preencher as estatísticas
            var stats = new OrderStatistics
            {
                TotalOrders = statusCounts.Values.Sum(),
                CreatedOrders = statusCounts.TryGetValue("Created", out var created) ? created : 0,
                InProgressOrders = statusCounts.TryGetValue("InProgress", out var inProgress) ? inProgress : 0,
                CompletedOrders = statusCounts.TryGetValue("Completed", out var completed) ? completed : 0,
                ApprovedOrders = statusCounts.TryGetValue("Approved", out var approved) ? approved : 0,
                RejectedOrders = statusCounts.TryGetValue("Rejected", out var rejected) ? rejected : 0
            };

            return stats;
        }
    }
}