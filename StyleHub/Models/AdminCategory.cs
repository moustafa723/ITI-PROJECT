namespace StyleHub.Models
{
    public class AdminCategory
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public IFormFile? Photo { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

        public string Back_Color { get; set; } = "#FFFFFF";
    }
}
