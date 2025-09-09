using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace StyleHub.Controllers
{
    [Authorize]  
    [Route("[controller]/[action]")]
    public class CheckoutController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7158/"); // Your API URL
        }

        private void AttachUserHeader()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
            if (!string.IsNullOrWhiteSpace(userId))
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        }

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

        [HttpPost, ActionName("Checkout")]
        public async Task<IActionResult> PlaceOrder(string customerName, string customerEmail)
        {
            AttachUserHeader();

            var cart = await _httpClient.GetFromJsonAsync<Cart>("api/cart/mine");
            if (cart == null || cart.CartItems.Count == 0)
                return BadRequest("Cart is empty");

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

            var response = await _httpClient.PostAsJsonAsync("api/Orders", order);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "❌ Failed to place order.";
                return RedirectToAction("Checkout");
            }

            await _httpClient.PostAsync("api/cart/mine/clear", null);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}