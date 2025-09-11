using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace StyleHub.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly HttpClient _httpClient;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7158/"); 
        }

        private void AttachUserHeader()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _httpClient.DefaultRequestHeaders.Remove("X-User-Id");
            if (!string.IsNullOrEmpty(userId))
                _httpClient.DefaultRequestHeaders.Add("X-User-Id", userId);
        }

        //  GET Checkout page
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            AttachUserHeader();

            var cart = await _httpClient.GetFromJsonAsync<Cart>("api/cart/mine");
            if (cart == null || !cart.CartItems.Any())
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
        public async Task<IActionResult> PlaceOrder(string cartId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // 🔴 Call API, do NOT use _context
            var response = await _httpClient.PostAsJsonAsync($"api/orders?userId={userId}", new { CartId = cartId });

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Failed to place order.";
                return RedirectToAction("Index");
            }

            var order = await response.Content.ReadFromJsonAsync<Order>();

            return RedirectToAction("Confirmation", new { id = order?.Id });
        }

        public IActionResult Confirmation(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }


    }
}
