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

            // الصور المرفوعة من الفورم
            public List<IFormFile> Images { get; set; } = new();

            public string? Alts { get; set; }

            // التصنيف المختار
            public int CategoryId { get; set; }

            // علشان نعرض Dropdown فيه كل التصنيفات
            public List<SelectListItem>? Categories { get; set; }
       
    }
}
