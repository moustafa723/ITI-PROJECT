using Microsoft.AspNetCore.Mvc.Rendering;

namespace StyleHubApi.models.Vm
{
    public class ProductVm
    {
        public string Name { get; set; } = string.Empty;

        public double Price { get; set; }

        public string? Size { get; set; }

        public int Review { get; set; }

        public bool InStock { get; set; }

        public string? Badge { get; set; }

        public double? OldPrice { get; set; }

        public double Rating { get; set; }

        public string? Color { get; set; }

        public List<IFormFile> Images { get; set; } = new();

        public string? Alts { get; set; }

        public int CategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }

        // ✳️ إضافات جديدة
        public bool? IsBestSeller { get; set; }

        public int? SalesCount { get; set; }

        public string? Description { get; set; }
            
        // اختياري
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}