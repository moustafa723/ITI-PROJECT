namespace StyleHubApi.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // نخزن اسم الصورة أو مسارها
        public string? PhotoPath { get; set; }

        public int Count_Product { get; set; }

        public string Back_Color { get; set; } = "#FFFFFF"; // لون افتراضي

        // العلاقة مع المنتجات
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
