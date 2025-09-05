using System.ComponentModel.DataAnnotations.Schema;

namespace StyleHubApi.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // نخزن اسم الصورة أو مسارها
        public string? PhotoPath { get; set; }
        public string Back_Color { get; set; } = "#FFFFFF"; // لون افتراضي

        // العلاقة مع المنتجات
        public ICollection<Product> Products { get; set; } = new List<Product>();
        [NotMapped] // مش هيتخزن في الداتا
        public int Count_Product => Products?.Count ?? 0;
    }
}
