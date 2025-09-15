using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace StyleHubApi.Models
{
    public class Product
    {
          public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public decimal? OldPrice { get; set; }
            public string? Size { get; set; }
            public string? Color { get; set; }
            public bool In_stock { get; set; }
            public string? Badge { get; set; }
            public double Rating { get; set; }
            public int Review { get; set; }
            public string? Alts { get; set; }
            public string? Description { get; set; } // جديد

            public bool? IsBestSeller { get; set; } // جديد
            public int? SalesCount { get; set; } // جديد
            public DateTime? CreatedAt { get; set; } = DateTime.UtcNow; // جديد
            public DateTime? UpdatedAt { get; set; } // جديد

            public List<string> Images { get; set; } = new();

            [ForeignKey(nameof(Category))]
            public int CategoryId { get; set; }

            // Navigation property
            public Category Category { get; set; }
    }
}

