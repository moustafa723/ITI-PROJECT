using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace StyleHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductController : Controller
    {
        private readonly HttpClient _http;

        public AdminProductController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7158/"); // API base
        }

        // GET: /AdminProduct
        public async Task<IActionResult> Index()
        {
            var products = await _http.GetFromJsonAsync<List<Product>>("api/Products");
            return View(products);
        }

        // GET: /AdminProduct/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AdminProduct/Create
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var response = await _http.PostAsJsonAsync("api/Products", product);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create product");
            return View(product);
        }

        // GET: /AdminProduct/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _http.GetFromJsonAsync<Product>($"api/Products/{id}");
            return View(product);
        }

        // POST: /AdminProduct/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            var response = await _http.PutAsJsonAsync($"api/Products/{id}", product);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update product");
            return View(product);
        }

        // GET: /AdminProduct/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _http.GetFromJsonAsync<Product>($"api/Products/{id}");
            return View(product);
        }

        // POST: /AdminProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _http.DeleteAsync($"api/Products/{id}");
            return RedirectToAction(nameof(Index));
        }
    }
}
