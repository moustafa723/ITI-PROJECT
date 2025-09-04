using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace StyleHubApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string? Size { get; set; }
        public int Review { get; set; }
        public bool In_stock { get; set; }
        public string? Badge { get; set; }
        public double? OldPrice { get; set; }
        public double Rating { get; set; }
        public string? Color { get; set; }
        public List<string> Images { get; set; } = new();
        public string? Alts { get; set; }
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        // Navigation property
        public Category Category { get; set; }
    }
}

