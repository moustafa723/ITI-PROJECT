using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StyleHub.Controllers
{
    public class AdminOrdersController : Controller
    {
        private readonly HttpClient _http;

        public AdminOrdersController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("StyleHubClient");
            _http.DefaultRequestHeaders.Add("X-User-Role", "Admin");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _http.GetAsync("/api/orders/all");
            if (!response.IsSuccessStatusCode)
                return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderViewModel>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus newStatus)
        {
            var payload = new { status = newStatus };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _http.PatchAsync($"/api/orders/{id}/status", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Status updated successfully.";
            else
                TempData["Error"] = "Failed to update status.";

            return RedirectToAction("Index");
        }
    }
}