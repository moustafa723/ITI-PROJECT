namespace StyleHub.Models
{
    public class ProductVmAdmin
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public int Review { get; set; }
        public bool InStock { get; set; }
        public string? Badge { get; set; }
        public decimal? OldPrice { get; set; }
        public double Rating { get; set; }
        public string? Color { get; set; }
        public List<IFormFile>? Images { get; set; } 
        public string? Alts { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
