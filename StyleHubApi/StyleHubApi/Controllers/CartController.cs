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

        // GET: api/cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ToListAsync();
        }

        // GET: api/cart/5
       
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
            {
                cart = new Cart { CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        // POST: api/cart
        [HttpPost]
        public async Task<ActionResult<Cart>> CreateCart(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCart), new { id = cart.Id }, cart);
        }

        // PUT: api/cart/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, Cart cart)
        {
            if (id != cart.Id)
                return BadRequest();

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Carts.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/cart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return NotFound();

            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Carts.Remove(cart);

            await _context.SaveChangesAsync();

            return NoContent();
        }




        // POST: api/cart/{cartId}/items
        [HttpPost("{cartId}/items")]
        public async Task<IActionResult> AddItemToCart(int cartId, [FromBody] CartItemdto dto)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            // 🔑 Auto-create a new cart if it doesn’t exist
            if (cart == null)
            {
                cart = new Cart { Id = cartId, CartItems = new List<CartItem>() };
                _context.Carts.Add(cart);
            }

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null) return NotFound("Product not found");

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == dto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Product = product
                });
            }

            await _context.SaveChangesAsync();
            return Ok(cart);
        }











        // DELETE: api/cart/{cartId}/items/{productId}
        [HttpDelete("{cartId}/items/{productId}")]
        public async Task<IActionResult> RemoveItemFromCart(int cartId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart == null)
                return NotFound("Cart not found");

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
            if (item == null)
                return NotFound("Item not found");

            cart.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/cart/{id}/clear
        [HttpPost("{id}/clear")]
        public async Task<IActionResult> ClearCart(int id)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cart == null)
                return NotFound("Cart not found");

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}
