using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleHubApi.Data;
using StyleHubApi.Models;
using System.Security.Claims;

namespace StyleHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CartItemController> _logger;

        public CartItemController(AppDbContext context, ILogger<CartItemController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/cartitem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartItem>>> GetCartItems()
        {
            try
            {
                var userId = GetUserId();
                var cart = await GetUserCart(userId);

                if (cart == null)
                {
                    return Ok(new List<CartItem>());
                }

                var cartItems = await _context.CartItems
                    .Include(ci => ci.Products)
                    .Where(ci => ci.CartId == cart.Id)
                    .ToListAsync();

                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart items");
                return StatusCode(500, "An error occurred while retrieving cart items");
            }
        }

        // GET: api/cartitem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartItem>> GetCartItem(int id)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(ci => ci.Products)
                    .FirstOrDefaultAsync(ci => ci.Id == id);

                if (cartItem == null)
                {
                    return NotFound();
                }

                // Verify ownership
                var userId = GetUserId();
                var cart = await GetUserCart(userId);

                if (cart == null || cartItem.CartId != cart.Id)
                {
                    return Forbid();
                }

                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart item with ID {CartItemId}", id);
                return StatusCode(500, "An error occurred while retrieving the cart item");
            }
        }

        // POST: api/cartitem
        [HttpPost]
        public async Task<ActionResult<CartItem>> CreateCartItem([FromBody] CartItem cartItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify product exists
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null)
                {
                    return NotFound("Product not found");
                }

                // Get or create user cart
                var userId = GetUserId();
                var cart = await GetOrCreateCart(userId);

                // Check if item already exists in cart
                var existingItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == cartItem.ProductId);

                if (existingItem != null)
                {
                    // Update quantity if item already exists
                    existingItem.Quantity += cartItem.Quantity;
                    _context.Entry(existingItem).State = EntityState.Modified;
                }
                else
                {
                    // Create new cart item
                    cartItem.CartId = cart.Id;
                    cartItem.Price = product.Price;
                    cartItem.ProductName = product.Name;
                    cartItem.ImageUrl = product.Images.FirstOrDefault();
                    cartItem.Quantity = cartItem.Quantity > 0 ? cartItem.Quantity : 1;


                    _context.CartItems.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                // Return the created/updated cart item with product details
                //var resultItem = await _context.CartItems
                //     .Include(ci => ci.Products)
                //     .FirstOrDefaultAsync(ci => ci.Id == (existingItem.Id ?? cartItem.Id));


                int? existingItemId = existingItem?.Id;
                int resultId = existingItemId ?? cartItem.Id;

                var resultItem = await _context.CartItems
                    .Include(ci => ci.Products)
                    .FirstOrDefaultAsync(ci => ci.Id == resultId);
                return CreatedAtAction(nameof(GetCartItem), new { id = resultItem.Id }, resultItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart item");
                return StatusCode(500, "An error occurred while creating the cart item");
            }
        }

        // PUT: api/cartitem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartItem(int id, [FromBody] CartItem cartItem)
        {
            try
            {
                if (id != cartItem.Id)
                {
                    return BadRequest("ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verify ownership
                var userId = GetUserId();
                var cart = await GetUserCart(userId);

                if (cart == null)
                {
                    return NotFound("Cart not found");
                }

                var existingItem = await _context.CartItems.FindAsync(id);
                if (existingItem == null)
                {
                    return NotFound();
                }

                if (existingItem.CartId != cart.Id)
                {
                    return Forbid();
                }

                // Update only the quantity (prevent changing other properties)
                existingItem.Quantity = cartItem.Quantity;
                _context.Entry(existingItem).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CartItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Concurrency error updating cart item with ID {CartItemId}", id);
                    return StatusCode(500, "A concurrency error occurred");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item with ID {CartItemId}", id);
                return StatusCode(500, "An error occurred while updating the cart item");
            }
        }

        // DELETE: api/cartitem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(id);
                if (cartItem == null)
                {
                    return NotFound();
                }

                // Verify ownership
                var userId = GetUserId();
                var cart = await GetUserCart(userId);

                if (cart == null || cartItem.CartId != cart.Id)
                {
                    return Forbid();
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart item with ID {CartItemId}", id);
                return StatusCode(500, "An error occurred while deleting the cart item");
            }
        }

        // DELETE: api/cartitem/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                var cart = await GetUserCart(userId);

                if (cart == null)
                {
                    return Ok("Cart is already empty");
                }

                var cartItems = _context.CartItems.Where(ci => ci.CartId == cart.Id);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();

                return Ok("Cart cleared successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, "An error occurred while clearing the cart");
            }
        }

        // GET: api/cartitem/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCartItemsCount()
        {
            try
            {
                var userId = GetUserId();
                var cart = await GetUserCart(userId);

                if (cart == null)
                {
                    return Ok(0);
                }

                var count = await _context.CartItems
                    .Where(ci => ci.CartId == cart.Id)
                    .SumAsync(ci => ci.Quantity);

                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart items count");
                return StatusCode(500, "An error occurred while getting cart items count");
            }
        }

        // GET: api/cartitem/total
        [HttpGet("total")]
        public async Task<ActionResult<decimal>> GetCartTotal()
        {
            try
            {
                var userId = GetUserId();
                var cart = await GetUserCart(userId);

                if (cart == null)
                {
                    return Ok(0);
                }

                var total = await _context.CartItems
                    .Where(ci => ci.Id == cart.Id)
                    .SumAsync(ci => ci.Price * ci.Quantity);

                return Ok(total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cart total");
                return StatusCode(500, "An error occurred while getting cart total");
            }
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.Id == id);
        }

        private string GetUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var sessionId = Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                Response.Cookies.Append("SessionId", sessionId, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    IsEssential = true,
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }

            return sessionId;
        }

        private async Task<Cart> GetUserCart(string userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        private async Task<Cart> GetOrCreateCart(string userId)
        {
            var cart = await GetUserCart(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }
    }
}