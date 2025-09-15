using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleHubApi.Data;
using StyleHubApi.Models;
using StyleHubApi.models.Vm;

namespace StyleHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
    [FromQuery] string? keyword,
    [FromQuery] string? sortBy,
    [FromQuery] string? sortDir = "asc")
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // البحث بالاسم
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Name.Contains(keyword));
            }

            // الفرز
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price":
                        query = sortDir == "desc"
                            ? query.OrderByDescending(p => p.Price)
                            : query.OrderBy(p => p.Price);
                        break;

                    case "rating":
                        query = sortDir == "desc"
                            ? query.OrderByDescending(p => p.Rating)
                            : query.OrderBy(p => p.Rating);
                        break;

                    case "name":
                        query = sortDir == "desc"
                            ? query.OrderByDescending(p => p.Name)
                            : query.OrderBy(p => p.Name);
                        break;
                }
            }

            return await query.ToListAsync();
        }

        // GET: api/products?categoryId=3
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery] int? categoryId)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var products = await query.ToListAsync();
            return products;
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return product;
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductVm productVm)
        {
            // رفع الصور
            var imagePaths = new List<string>();
            if (productVm.Images != null && productVm.Images.Any())
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var image in productVm.Images)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    imagePaths.Add($"{baseUrl}/uploads/{fileName}");
                }
            }

            var product = new Product
            {
                Name = productVm.Name,
                Price = (decimal)productVm.Price,
                Size = productVm.Size,
                Review = productVm.Review,
                In_stock = productVm.InStock,
                Badge = productVm.Badge,
                OldPrice = (decimal?)productVm.OldPrice,
                Rating = productVm.Rating,
                Color = productVm.Color,
                Images = imagePaths,
                Alts = productVm.Alts,
                CategoryId = productVm.CategoryId,
                IsBestSeller = productVm.IsBestSeller,
                SalesCount = productVm.SalesCount,
                Description = productVm.Description,
                CreatedAt = DateTime.UtcNow,  // يمكن تحديده تلقائيًا
                UpdatedAt = null
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductVm productVm)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Name = productVm.Name;
            product.Price = (decimal)productVm.Price;
            product.Size = productVm.Size;
            product.Review = productVm.Review;
            product.In_stock = productVm.InStock;
            product.Badge = productVm.Badge;
            product.OldPrice = (decimal?)productVm.OldPrice;
            product.Rating = productVm.Rating;
            product.Color = productVm.Color;
            product.Alts = productVm.Alts;
            product.CategoryId = productVm.CategoryId;
            product.IsBestSeller = productVm.IsBestSeller;
            product.SalesCount = productVm.SalesCount;
            product.Description = productVm.Description;
            product.UpdatedAt = DateTime.UtcNow;

            // تحديث الصور لو فيه صور جديدة
            if (productVm.Images != null && productVm.Images.Any())
            {
                var imagePaths = new List<string>();
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var image in productVm.Images)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    imagePaths.Add($"{baseUrl}/uploads/{fileName}");
                }

                product.Images = imagePaths;
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
