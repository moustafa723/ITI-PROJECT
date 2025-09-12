using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleHubApi.Data;
using StyleHubApi.models.DTO;
using StyleHubApi.Models;

namespace StyleHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public OrdersController(AppDbContext context) => _context = context;

        private string? GetUserId() =>
            Request.Headers.TryGetValue("X-User-Id", out var h) ? h.ToString() : null;

        // GET: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderDto dto)
        {
            var userId = GetUserId();
            if (userId is null) return BadRequest("Missing user ID");

            var order = new Order
            {
                UserId = userId,
                CustomerName = dto.CustomerName,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                AddressId = dto.AddressId,
                OrderItems = dto.OrderItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }


        // GET: api/orders/mine
        [HttpGet("mine")]
        public async Task<ActionResult<List<Order>>> GetMyOrders()
        {
            var userId = GetUserId();
            if (userId is null) return BadRequest("Missing user ID");

            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
