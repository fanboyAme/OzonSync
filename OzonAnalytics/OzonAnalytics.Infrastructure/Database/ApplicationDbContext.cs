using Microsoft.EntityFrameworkCore;
using OzonAnalytics.Domain.Entities;

namespace OzonAnalytics.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<SyncLog> SyncLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(ap => ap.NormalizedEmail)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasOne<Shop>()
                .WithMany()
                .HasForeignKey(o => o.ShopId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.ShopId, o.OzonOrderId })
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.TokenHash);
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId);

            modelBuilder.Entity<OrderItem>()
                .HasOne<Order>()
                .WithMany()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OrderItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shop>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Shop>()
                .HasIndex(s => new { s.UserId, s.OzonClientId })
                .IsUnique();

            modelBuilder.Entity<SyncLog>()
               .HasOne<Shop>()
               .WithMany()
               .HasForeignKey(sl => sl.ShopId)
               .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SyncLog>()
                .HasIndex(sl => sl.ShopId);

            modelBuilder.Entity<Product>()
              .HasOne<Shop>()
              .WithMany()
              .HasForeignKey(p => p.ShopId)
              .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.ShopId, p.OzonSku })
                .IsUnique();
        }

    }
}
