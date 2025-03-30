using Microsoft.EntityFrameworkCore;
using OSManager.Models;

namespace OSManager.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            // Verificar se já existem dados
            if (await context.Users.AnyAsync())
                return;  // DB já foi populado

            // Criar usuários
            var admin = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@exemplo.com",
                FullName = "Administrador do Sistema",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var supervisor = new User
            {
                Username = "supervisor",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("supervisor123"),
                Email = "supervisor@exemplo.com",
                FullName = "Supervisor de Serviços",
                Role = "Supervisor",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var tecnico = new User
            {
                Username = "tecnico",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("tecnico123"),
                Email = "tecnico@exemplo.com",
                FullName = "Técnico de Campo",
                Role = "Technician",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await context.Users.AddRangeAsync(admin, supervisor, tecnico);
            await context.SaveChangesAsync();

            // Criar templates de checklist organizados por categorias
            var checklistTemplates = new List<ChecklistItem>
            {
                // Categoria: Preparação
                new ChecklistItem { Description = "Verificar equipamentos necessários", Category = "Preparação", DisplayOrder = 1, IsTemplate = true },
                new ChecklistItem { Description = "Inspecionar EPI's", Category = "Preparação", DisplayOrder = 2, IsTemplate = true },
                new ChecklistItem { Description = "Conferir documentação", Category = "Preparação", DisplayOrder = 3, IsTemplate = true },
                
                // Categoria: Inspeção
                new ChecklistItem { Description = "Realizar inspeção visual do local", Category = "Inspeção", DisplayOrder = 1, IsTemplate = true },
                new ChecklistItem { Description = "Verificar condições de segurança", Category = "Inspeção", DisplayOrder = 2, IsTemplate = true },
                new ChecklistItem { Description = "Identificar riscos potenciais", Category = "Inspeção", DisplayOrder = 3, IsTemplate = true },
                
                // Categoria: Execução
                new ChecklistItem { Description = "Executar procedimento técnico", Category = "Execução", DisplayOrder = 1, IsTemplate = true },
                new ChecklistItem { Description = "Testar funcionamento", Category = "Execução", DisplayOrder = 2, IsTemplate = true },
                new ChecklistItem { Description = "Ajustar configurações", Category = "Execução", DisplayOrder = 3, IsTemplate = true },
                
                // Categoria: Finalização
                new ChecklistItem { Description = "Limpar área de trabalho", Category = "Finalização", DisplayOrder = 1, IsTemplate = true },
                new ChecklistItem { Description = "Listar materiais utilizados", Category = "Finalização", DisplayOrder = 2, IsTemplate = true },
                new ChecklistItem { Description = "Documentar alterações realizadas", Category = "Finalização", DisplayOrder = 3, IsTemplate = true }
            };

            await context.ChecklistItems.AddRangeAsync(checklistTemplates);
            await context.SaveChangesAsync();

            // Opcionalmente adicionar algumas ordens de exemplo para demonstração
            if (context.Database.IsSqlServer() && context.Database.GetConnectionString()?.Contains("Development") == true)
            {
                // Somente cria dados de exemplo em ambiente de desenvolvimento
                await CreateSampleOrdersAsync(context, tecnico.Id, supervisor.Id);
            }
        }

        private static async Task CreateSampleOrdersAsync(AppDbContext context, int tecnicoId, int supervisorId)
        {
            // Criar algumas ordens de exemplo em diferentes estados
            var ordens = new List<Order>
            {
                new Order
                {
                    Description = "Manutenção preventiva em servidor",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    Status = "Created",
                    UserId = tecnicoId
                },
                new Order
                {
                    Description = "Instalação de novo roteador",
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    StartedAt = DateTime.UtcNow.AddDays(-6),
                    Status = "InProgress",
                    UserId = tecnicoId
                },
                new Order
                {
                    Description = "Substituição de peças em equipamento",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    StartedAt = DateTime.UtcNow.AddDays(-4),
                    CompletedAt = DateTime.UtcNow.AddDays(-2),
                    Status = "Completed",
                    UserId = tecnicoId
                },
                new Order
                {
                    Description = "Instalação de nova impressora",
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    StartedAt = DateTime.UtcNow.AddDays(-14),
                    CompletedAt = DateTime.UtcNow.AddDays(-12),
                    ApprovedAt = DateTime.UtcNow.AddDays(-11),
                    Status = "Approved",
                    UserId = tecnicoId,
                    ApproverId = supervisorId
                },
                new Order
                {
                    Description = "Reparo em cabeamento de rede",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    StartedAt = DateTime.UtcNow.AddDays(-19),
                    CompletedAt = DateTime.UtcNow.AddDays(-18),
                    ApprovedAt = DateTime.UtcNow.AddDays(-17),
                    Status = "Rejected",
                    RejectionReason = "Necessário refazer alguns pontos de rede",
                    UserId = tecnicoId,
                    ApproverId = supervisorId
                }
            };

            await context.Orders.AddRangeAsync(ordens);
            await context.SaveChangesAsync();

            // Adicionar checklist items às orders
            var templates = await context.ChecklistItems
                .Where(c => c.IsTemplate)
                .ToListAsync();

            foreach (var ordem in ordens)
            {
                // Criar checklist items com base nos templates
                var checklistItems = templates.Select(t => new ChecklistItem
                {
                    Description = t.Description,
                    Category = t.Category,
                    DisplayOrder = t.DisplayOrder,
                    IsTemplate = false,
                    OrderId = ordem.Id,
                    // Para ordens completed/approved, marcar tudo como concluído
                    IsCompleted = ordem.Status == "Completed" || ordem.Status == "Approved" || ordem.Status == "Rejected"
                }).ToList();

                await context.ChecklistItems.AddRangeAsync(checklistItems);
            }

            await context.SaveChangesAsync();
        }
    }
}