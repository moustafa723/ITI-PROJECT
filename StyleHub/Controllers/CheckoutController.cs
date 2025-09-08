using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace StyleHub.Controllers
{
    [Route("[controller]/[action]")]
    public class CheckoutController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44374/"); // Your API URL
        }
        private void AttachUserHeader()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
            if (!string.IsNullOrWhiteSpace(userId))
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        }

        // GET: /Checkout
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AttachUserHeader();

            var cart = await _httpClient.GetFromJsonAsync<Cart>("api/cart/mine");
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            return View(cart);
        }


        // POST: /Checkout
        [HttpPost, ActionName("Checkout")]
        public async Task<IActionResult> PlaceOrder(string customerName, string customerEmail)
        {
            AttachUserHeader();

            // 1. Get cart
            var cart = await _httpClient.GetFromJsonAsync<Cart>("api/cart/mine");
            if (cart == null || cart.CartItems.Count == 0)
                return BadRequest("Cart is empty");

            // 2. Build order
            var order = new Order
            {
                CustomerName = customerName,
                Email = customerEmail,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product?.Price ?? 0
                }).ToList()
            };

            // 3. Send order to API
            var response = await _httpClient.PostAsJsonAsync("api/Orders", order);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "❌ Failed to place order.";
                return RedirectToAction("Checkout");
            }

            // 4. Clear the cart
            await _httpClient.PostAsync("api/cart/mine/clear", null);

            return RedirectToAction("Success");
        }


        // GET: /Checkout/Success
        public IActionResult Success()
        {
            return View();
        }
    }
}
