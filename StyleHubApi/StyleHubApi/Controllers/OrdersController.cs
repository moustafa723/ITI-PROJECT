using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleHubApi.Data;
using StyleHubApi.Models;

namespace StyleHubApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        //  Get current user's orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders([FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "Missing userId" });

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .ToListAsync();

            return Ok(orders);
        }

        //  Place a new order
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(Order order, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "Missing userId" });

            order.UserId = userId;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        //  Update order status
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updated, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "Missing userId" });

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order == null) return NotFound();

            order.Status = updated.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //  Delete an order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id, [FromQuery] string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest(new { message = "Missing userId" });

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
