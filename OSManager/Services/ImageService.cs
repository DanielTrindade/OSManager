using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OSManager.Data;
using OSManager.Models;

namespace OSManager.Services
{
    public class ImageService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ImageService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageService(
        AppDbContext context,
        IWebHostEnvironment environment,
        ILogger<ImageService> logger,
        IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Image?> SaveImageAsync(IFormFile file, int orderId, int? checklistItemId = null)
        {
            // Verificar se a ordem existe
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return null;

            // Se tiver checklistItemId, verificar se o item pertence a esta ordem
            if (checklistItemId.HasValue)
            {
                var checklistItem = await _context.ChecklistItems
                    .FirstOrDefaultAsync(c => c.Id == checklistItemId.Value && c.OrderId == orderId);

                if (checklistItem == null)
                    return null;
            }

            // Validar o arquivo
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
            if (!allowedTypes.Contains(file.ContentType))
                return null;

            // Limitar o tamanho do arquivo (5MB)
            if (file.Length > 5 * 1024 * 1024)
                return null;

            // Gerar nome único para o arquivo
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            // Configurar o diretório de uploads
            var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Salvar o arquivo
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Criar e salvar o registro da imagem
            var image = new Image
            {
                FileName = fileName,
                FilePath = filePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                OrderId = orderId,
                ChecklistItemId = checklistItemId
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return image;
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            // Obter o usuário atual do HttpContext
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
                return false;

            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdminOrSupervisor = user.IsInRole("Admin") || user.IsInRole("Supervisor");

            // Consulta base
            var query = _context.Images.Include(i => i.Order);

            // Buscar a imagem - com verificação de autorização já incorporada
            Image? image;
            if (isAdminOrSupervisor)
            {
                // Admins/Supervisores podem deletar qualquer imagem
                image = await query.FirstOrDefaultAsync(i => i.Id == id);
            }
            else
            {
                // Usuários comuns só podem deletar suas próprias imagens
                image = await query.FirstOrDefaultAsync(i => i.Id == id &&
                                                           i.Order != null &&
                                                           i.Order.UserId == userId);
            }

            if (image == null)
                return false;

            // Apagar arquivo físico
            if (File.Exists(image.FilePath))
            {
                try
                {
                    File.Delete(image.FilePath);
                }
                catch (IOException ex)
                {
                    _logger.LogWarning(ex, $"Não foi possível excluir o arquivo físico: {image.FilePath}");
                }
            }

            // Remover registro do banco
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}