using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;

namespace StyleHub.Controllers
{
    [Route("[controller]/[action]")]
    public class CheckoutController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7158/"); // Your API URL
        }

        // GET: /Checkout
        [HttpGet]
        public async Task<IActionResult> Index(int cartId = 1)
        {
            var cart = await _httpClient.GetFromJsonAsync<Cart>($"api/Cart/{cartId}");
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart", new { cartId });
            }

            return View(cart); // 👈 sends Cart model to Checkout.cshtml
        }

        // POST: /Checkout
        [HttpPost, ActionName("Checkout")]
        public async Task<IActionResult> PlaceOrder(int cartId, string customerName, string customerEmail)
        {
            // 1. Get cart from API
            var cart = await _httpClient.GetFromJsonAsync<Cart>($"api/Cart/{cartId}");
            if (cart == null || cart.CartItems.Count == 0)
                return BadRequest("Cart is empty");

            // 2. Build order object
            var order = new Order
            {
                CustomerName = customerName,
                Email = customerEmail,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                }).ToList()
            };

            // 3. Send order to API
            var response = await _httpClient.PostAsJsonAsync("api/Orders", order);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "❌ Failed to place order.";
                return RedirectToAction("Checkout", new { cartId });
            }

            // 4. Clear the cart
            await _httpClient.DeleteAsync($"api/Cart/{cartId}");

            return RedirectToAction("Success");
        }

        // GET: /Checkout/Success
        public IActionResult Success()
        {
            return View();
        }
    }
}
