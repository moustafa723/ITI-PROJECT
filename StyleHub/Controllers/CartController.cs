using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;

namespace StyleHub.Controllers
{
    public class CartController : Controller
    {
        private readonly HttpClient _httpClient;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44374/"); // Your API URL
        }


      



        // Show cart
        public async Task<IActionResult> Index(int cartId = 1) // testing with cart 1
        {
            var response = await _httpClient.GetAsync($"api/Cart/{cartId}");
            if (!response.IsSuccessStatusCode)
                return View(new Cart { CartItems = new List<Cart_Item>() });

            var cart = await _httpClient.GetFromJsonAsync<Cart>($"api/Cart/{cartId}");
            if (cart.CartItems == null) // ✅ prevent null reference
                cart.CartItems = new List<Cart_Item>();
            return View(cart);
        }

        // Add item
        [HttpPost]
        public async Task<IActionResult> AddToCart(int cartId, int productId, int quantity = 1)
        {
            var newItem = new Cart_Item
            {
                ProductId = productId,

                Quantity = quantity
            };

            var response = await _httpClient.PostAsJsonAsync($"api/Cart/{cartId}/items", newItem);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", new { cartId });

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest("Failed to add item: " + error);
        }

        // Remove item
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId, int productId)
        {
            var response = await _httpClient.DeleteAsync($"api/Cart/{cartId}/items/{productId}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", new { cartId });

            var error = await response.Content.ReadAsStringAsync();
            return BadRequest("Failed to remove item: " + error);
        }
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartId, int productId)
        {
            var response = await _httpClient.DeleteAsync($"api/Cart/{cartId}/items/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "❌ Failed to remove item from cart.";
            }

            return RedirectToAction("Index", new { cartId });
        }



        [HttpPost]
        public async Task<IActionResult> ClearCart(int id)
        {
            var response = await _httpClient.PostAsync($"/api/cart/{id}/clear", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to clear cart";
            }

            return RedirectToAction("Index", new { id });
        }

    }
}