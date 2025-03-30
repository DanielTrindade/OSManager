using Microsoft.EntityFrameworkCore;
using OSManager.Models;

namespace OSManager.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
        public DbSet<Image> Images => Set<Image>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações do modelo User
            modelBuilder.Entity<User>()
                .HasMany<Order>()
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<User>()
                .HasMany<Order>()
                .WithOne(o => o.Approver)
                .HasForeignKey(o => o.ApproverId)
                .IsRequired(false);

            // Configurações do modelo Order
            modelBuilder.Entity<Order>()
                .HasMany(o => o.ChecklistItems)
                .WithOne(c => c.Order)
                .HasForeignKey(c => c.OrderId)
                .IsRequired(false);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Images)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);

            // Configurações do modelo ChecklistItem
            modelBuilder.Entity<ChecklistItem>()
                .HasMany(c => c.Images)
                .WithOne(i => i.ChecklistItem)
                .HasForeignKey(i => i.ChecklistItemId)
                .IsRequired(false);
            
        }
    }
}