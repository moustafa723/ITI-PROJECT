using StyleHub.Models;
namespace StyleHub.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoPath { get; set; }
        public string Back_Color { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
    }
}