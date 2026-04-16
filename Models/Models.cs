using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ZiyaCan.Models
{
    // ── Extended Identity User ──────────────────────────────
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)] public string FullName  { get; set; } = "";
        [MaxLength(300)] public string? Address  { get; set; }
        public double? Latitude  { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Caterer? CatererProfile { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    // ── Caterer ─────────────────────────────────────────────
    public class Caterer
    {
        public int Id { get; set; }
        [Required, MaxLength(150)] public string BusinessName { get; set; } = "";
        [MaxLength(500)]           public string? Description { get; set; }
        [MaxLength(300)]           public string? Address     { get; set; }
        public double Latitude  { get; set; }
        public double Longitude { get; set; }
        public bool IsApproved  { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = "";
        public ApplicationUser User { get; set; } = null!;

        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public ICollection<Order>    Orders    { get; set; } = new List<Order>();
    }

    // ── MenuItem ────────────────────────────────────────────
    public class MenuItem
    {
        public int Id { get; set; }
        [Required, MaxLength(200)] public string Title       { get; set; } = "";
        [MaxLength(1000)]          public string? Description { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal Price { get; set; }
        [MaxLength(300)] public string? ImagePath { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CatererId { get; set; }
        public Caterer Caterer { get; set; } = null!;

        public ICollection<CustomizationGroup> CustomizationGroups { get; set; } = new List<CustomizationGroup>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    // ── Customization ───────────────────────────────────────
    public class CustomizationGroup
    {
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = ""; // e.g. "Sauces"
        public bool AllowMultiple { get; set; } = false;

        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;

        public ICollection<CustomizationOption> Options { get; set; } = new List<CustomizationOption>();
    }

    public class CustomizationOption
    {
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Label { get; set; } = ""; // e.g. "Extra Cheese"
        public OptionType Type { get; set; } = OptionType.Addition;
        [Column(TypeName = "decimal(10,2)")] public decimal PriceDelta { get; set; } = 0;

        public int CustomizationGroupId { get; set; }
        public CustomizationGroup Group { get; set; } = null!;
    }

    public enum OptionType { Removal = 0, Addition = 1, Choice = 2 }

    // ── Order ───────────────────────────────────────────────
    public class Order
    {
        public int Id { get; set; }
        [MaxLength(20)] public string OrderCode { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [Column(TypeName = "decimal(10,2)")] public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        public string UserId { get; set; } = "";
        public ApplicationUser User { get; set; } = null!;

        public int CatererId { get; set; }
        public Caterer Caterer { get; set; } = null!;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }

    public enum OrderStatus { Pending, Confirmed, Preparing, OnTheWay, Delivered, Completed, Cancelled }

    // ── OrderItem ───────────────────────────────────────────
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        [Column(TypeName = "decimal(10,2)")] public decimal UnitPrice { get; set; }
        [MaxLength(500)] public string? CustomizationSnapshot { get; set; } // JSON

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;
    }

    // ── Payment ─────────────────────────────────────────────
    public class Payment
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(10,2)")] public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        [MaxLength(50)] public string? TransactionCode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }

    public enum PaymentStatus { Pending, Success, Failed }

    // ── Ratings ─────────────────────────────────────────────
    public class CatererRating
    {
        public int Id { get; set; }
        [Range(1, 5)] public int Score { get; set; }
        [MaxLength(500)] public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CatererId { get; set; }
        public Caterer Caterer { get; set; } = null!;

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }

    public class MenuItemRating
    {
        public int Id { get; set; }
        [Range(1, 5)] public int Score { get; set; }
        [MaxLength(500)] public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; } = null!;

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
    }

    // ── Log ─────────────────────────────────────────────────
    public class Log
    {
        public int Id { get; set; }
        public LogLevel Level { get; set; } = LogLevel.Info;
        [Required, MaxLength(300)] public string Message { get; set; } = "";
        [MaxLength(50)] public string? IpAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }

    public enum LogLevel { Info, Warning, Error }
}
