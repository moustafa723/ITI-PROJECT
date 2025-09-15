using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StyleHub.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly HttpClient _httpClient;

        public OrdersController(
            ILogger<OrdersController> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("StyleHubClient");
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("");
            if (!response.IsSuccessStatusCode)
                return View(new List<Order>());

            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(orders);
        }
    }
}