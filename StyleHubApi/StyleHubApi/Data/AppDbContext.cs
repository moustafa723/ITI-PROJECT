using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StyleHubApi.models;
using StyleHubApi.Models;
using System.Linq;
using System.Text.Json;

namespace StyleHubApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------- Cart --------
            modelBuilder.Entity<Cart>(e =>
            {
                e.HasKey(c => c.Id);

                // سلة واحدة لكل يوزر (لما UserId يكون مش null)
                e.HasIndex(c => c.UserId)
                 .IsUnique()
                 .HasFilter("UserId IS NOT NULL"); // SQLite: الفلتر بدون أقواس
            });

            // -------- CartItem --------
            modelBuilder.Entity<CartItem>(e =>
            {
                e.HasKey(ci => ci.Id);

                // منع تكرار نفس المنتج داخل نفس السلة
                e.HasIndex(ci => new { ci.CartId, ci.ProductId }).IsUnique();

                e.HasOne(ci => ci.Cart)
                 .WithMany(c => c.CartItems)
                 .HasForeignKey(ci => ci.CartId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(ci => ci.Product)
                 .WithMany()
                 .HasForeignKey(ci => ci.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);

                // (اختياري) SQLite لا يدعم precision قوي للـdecimal، التحويل لـdouble بيخلّي التخزين مستقر
                e.Property(ci => ci.Price).HasConversion<double>();
            });

            // -------- Product --------
            // خزن List<string> Images كـ JSON في عمود TEXT
            var imagesConverter = new ValueConverter<List<string>, string>(
     v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
     v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>()
 );
    var imagesComparer = new ValueComparer<List<string>>(
    (l1, l2) => l1 != null && l2 != null ? l1.SequenceEqual(l2) : l1 == l2,
    l => l == null ? 0 : l.Aggregate(0, (a, s) => HashCode.Combine(a, s ?? "")),
    l => l == null ? new List<string>() : new List<string>(l)
);




            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);

                e.HasOne(p => p.Category)
                 .WithMany(c => c.Products) // ✅ دي اللي محتاجينها فقط
                 .HasForeignKey(p => p.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.Property(p => p.Images)
                 .HasConversion(imagesConverter)
                 .Metadata.SetValueComparer(imagesComparer);

                e.Property(p => p.Images).HasColumnType("TEXT");
            });


            // -------- علاقات إضافية (اختياري حسب موديلاتك) --------
            // مثال لو Order/OrderItem/Payment محتاجين ضبط، بس ممكن تسيبهم لو شغالين.
            // modelBuilder.Entity<OrderItem>(...);
            // modelBuilder.Entity<Payment>(...);
        }
    }
}
