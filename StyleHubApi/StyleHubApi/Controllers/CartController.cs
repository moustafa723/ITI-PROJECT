using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleHubApi.Data;
using StyleHubApi.Models;
using StyleHubApi.models.DTO;

namespace StyleHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }
        // Helper: هات اليوزر من الهيدر أو الكويري
        // Helper
        private string? GetUserId() =>
            Request.Headers.TryGetValue("X-User-Id", out var h) ? h.ToString()
            : (Request.Query.TryGetValue("userId", out var q) ? q.ToString() : null);

        // GET: api/cart/mine
        [HttpGet("mine")]
        public async Task<ActionResult<Cart>> GetMyCart()
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId (header or query)");

            var cart = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == uid);

            if (cart == null)
            {
                cart = new Cart { UserId = uid, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // POST: api/cart/mine/items
        [HttpPost("mine/items")]
        public async Task<IActionResult> AddItemToMyCart([FromBody] CartItemdto dto)
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");
            if (dto == null || dto.Quantity <= 0) return BadRequest("Quantity must be > 0");

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) return NotFound("Product not found");

            var cart = await _context.Carts.Include(c => c.CartItems)
                          .FirstOrDefaultAsync(c => c.UserId == uid);

            if (cart == null)
            {
                cart = new Cart { UserId = uid, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // لازم قبل إضافة CartItems
            }

            var existing = cart.CartItems.FirstOrDefault(x => x.ProductId == dto.ProductId);
            if (existing != null) existing.Quantity += dto.Quantity;
            else _context.CartItems.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Price = product.Price
            });

            await _context.SaveChangesAsync();

            var result = await _context.Carts
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == cart.Id);

            return Ok(result);
        }

        // PUT: api/cart/mine/items/{productId}
        [HttpPut("mine/items/{productId}")]
        public async Task<IActionResult> UpdateMyItem(int productId, [FromBody] UpdateQuantityDto dto)
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");
            if (dto.Quantity < 0) return BadRequest("Quantity must be >= 0");

            var cart = await _context.Carts.Include(c => c.CartItems)
                         .FirstOrDefaultAsync(c => c.UserId == uid);
            if (cart == null) return NotFound("Cart not found");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return NotFound("Item not found");

            if (dto.Quantity == 0) _context.CartItems.Remove(item);
            else item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/cart/mine/items/{productId}
        [HttpDelete("mine/items/{productId}")]
        public async Task<IActionResult> RemoveMyItem(int productId)
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");

            var cart = await _context.Carts.Include(c => c.CartItems)
                         .FirstOrDefaultAsync(c => c.UserId == uid);
            if (cart == null) return NotFound("Cart not found");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return NotFound("Item not found");

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/cart/mine/clear
        [HttpPost("mine/clear")]
        public async Task<IActionResult> ClearMyCart()
        {
            var uid = GetUserId();
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing userId");

            var cart = await _context.Carts.Include(c => c.CartItems)
                         .FirstOrDefaultAsync(c => c.UserId == uid);
            if (cart == null) return NotFound("Cart not found");

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public record UpdateQuantityDto(int Quantity);




    }
}
