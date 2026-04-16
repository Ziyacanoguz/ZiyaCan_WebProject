using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZiyaCan.Models;

namespace ZiyaCan.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Caterer>             Caterers             { get; set; }
        public DbSet<MenuItem>            MenuItems            { get; set; }
        public DbSet<CustomizationGroup>  CustomizationGroups  { get; set; }
        public DbSet<CustomizationOption> CustomizationOptions { get; set; }
        public DbSet<Order>               Orders               { get; set; }
        public DbSet<OrderItem>           OrderItems           { get; set; }
        public DbSet<Payment>             Payments             { get; set; }
        public DbSet<CatererRating>       CatererRatings       { get; set; }
        public DbSet<MenuItemRating>      MenuItemRatings      { get; set; }
        public DbSet<Log>                 Logs                 { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Caterer <-> User (1-to-1)
            builder.Entity<Caterer>()
                .HasOne(c => c.User)
                .WithOne(u => u.CatererProfile)
                .HasForeignKey<Caterer>(c => c.UserId);

            // Order -> User (no cascade on delete)
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order -> Caterer
            builder.Entity<Order>()
                .HasOne(o => o.Caterer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CatererId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment (1-to-1 with Order)
            builder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId);

            // CatererRating -> Order (1-to-1)
            builder.Entity<CatererRating>()
                .HasIndex(r => r.OrderId).IsUnique();

            // Log -> User (nullable)
            builder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Seed roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin",   NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Caterer", NormalizedName = "CATERER" },
                new IdentityRole { Id = "3", Name = "User",    NormalizedName = "USER" }
            );
        }
    }
}
