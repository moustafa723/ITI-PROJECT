using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace StyleHub.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class OrdersController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrdersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7158/"); // your API URL
        }

        // GET: Orders/Order
        public async Task<IActionResult> Order()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var orders = await _httpClient.GetFromJsonAsync<List<Order>>($"api/Orders?userId={userId}");

            return View("Order",orders);
        }
    }
}
