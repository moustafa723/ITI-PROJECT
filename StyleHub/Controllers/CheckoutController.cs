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
            var addresses = await _httpClient.GetFromJsonAsync<List<AddressDto>>("api/addresses/mine")
                                        ?? new List<AddressDto>();
            var def = addresses.FirstOrDefault(a => a.IsDefault) ?? addresses.FirstOrDefault();

            var vm = new CheckoutVm { Cart = cart, DefaultAddress = def };
            return View(vm);
        }

        [HttpPost, ActionName("Checkout")]
        public async Task<IActionResult> PlaceOrder(string customerName, string customerEmail, string address, string? apartment, string city, string state, string zip, string country,
            bool? saveAddress, bool? billingSame)
        {
            AttachUserHeader();

            var cart = await _httpClient.GetFromJsonAsync<Cart>("api/cart/mine");
            if (cart == null || cart.CartItems.Count == 0)
                return BadRequest("Cart is empty");
              if (saveAddress == true)
            {
                var addrDto = new AddressDto
                {
                    Label = "Checkout",
                    Line1 = address,
                    Line2 = apartment,
                    City = city,
                    State = state,
                    PostalCode = zip,
                    Country = country,
                    // ContactName/Phone لو عندك حقول في الفورم
                };

                var addrRes = await _httpClient.PostAsJsonAsync("api/addresses/mine", addrDto);
                // مش هنوقف الأوردر لو فشل حفظ العنوان، لكن ممكن تسجل رسالة لو حابب
            }
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