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
        private readonly ILogger<CheckoutController> _logger;
        private readonly HttpClient _httpClient;

        public CheckoutController(
            ILogger<CheckoutController> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("StyleHubClient");
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
            var addresses = await _httpClient.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine")
                                        ?? new List<AddressDto>();
            var def = addresses.FirstOrDefault(a => a.IsDefault) ?? addresses.FirstOrDefault();

            var vm = new CheckoutVm { Cart = cart, DefaultAddress = def };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            AttachUserHeader();

            var cart = await _httpClient.GetFromJsonAsync<Cart>("api/cart/mine");
            if (cart == null || cart.CartItems.Count == 0)
                return BadRequest("Cart is empty");

            var addresses = await _httpClient.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine");
            var defaultAddress = addresses?.FirstOrDefault(a => a.IsDefault) ?? addresses?.FirstOrDefault();

            if (defaultAddress == null)
            {
                TempData["Error"] = "❌ No shipping address found. Please add one first.";
                return RedirectToAction("Index", "Account");
            }

            var userName = User.FindFirst("given_name")?.Value ?? "Customer";
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "unknown@example.com";

            var order = new Order
            {
                CustomerName = userName,
                Email = userEmail,
                AddressId = defaultAddress.Id,

                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = (int)ci.Quantity,
                    Price = ci.Product?.Price ?? 0
                }).ToList()
            };

            var response = await _httpClient.PostAsJsonAsync("api/orders", order);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "❌ Failed to place order.";
                return RedirectToAction("Checkout");
            }

            foreach (var item in cart.CartItems)
            {
                await _httpClient.DeleteAsync($"api/cart/mine/items/{item.ProductId}");
            }
            return RedirectToAction("Index", "Account", new { fragment = "orders" });
        }
    }
}