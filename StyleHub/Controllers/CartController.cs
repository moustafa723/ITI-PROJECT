using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;

namespace StyleHub.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7158/");
        }

        private void AttachUserHeader()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine("➡️ X-User-Id = " + userId); 
            _httpClient.DefaultRequestHeaders.Remove("X-User-Id");

            if (!string.IsNullOrWhiteSpace(userId))
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        }

       
        public async Task<IActionResult> Index()
        {
            AttachUserHeader();

            var response = await _httpClient.GetAsync("api/cart/mine");
            if (!response.IsSuccessStatusCode)
                return View(new Cart { CartItems = new List<Cart_Item>() });

            var cart = await response.Content.ReadFromJsonAsync<Cart>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            );

            if (cart?.CartItems == null)
                cart.CartItems = new List<Cart_Item>();

            return View(cart);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            AttachUserHeader();

            var newItem = new Cart_Item
            {
                ProductId = productId,
                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync("api/cart/mine/items", newItem);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest("❌ Failed to add item: " + error);
        }

        // حذف منتج
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            AttachUserHeader();

            var response = await _httpClient.DeleteAsync($"api/cart/mine/items/{productId}");

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "❌ Failed to remove item from cart.";

            return RedirectToAction("Index");
        }

        // مسح كل السلة
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            AttachUserHeader();

            var response = await _httpClient.PostAsync("api/cart/mine/clear", null);

            if (!response.IsSuccessStatusCode)
                TempData["Error"] = "❌ Failed to clear cart.";

            return RedirectToAction("Index");
        }
    }
}
