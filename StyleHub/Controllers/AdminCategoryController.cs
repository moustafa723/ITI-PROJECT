using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;

namespace StyleHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminCategoryController : Controller
    {
        private readonly HttpClient _http;

        public AdminCategoryController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("https://localhost:7158/"); // ✅ Change to your API base URL
        }

        // GET: /AdminCategory
        public async Task<IActionResult> Index()
        {
            var categories = await _http.GetFromJsonAsync<List<Category>>("api/Categories");
            return View(categories);
        }

        // GET: /AdminCategory/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AdminCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                var response = await _http.PostAsJsonAsync("api/Categories", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to create category");
            }
            return View(category);
        }

        // GET: /AdminCategory/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _http.GetFromJsonAsync<Category>($"api/Categories/{id}");
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: /AdminCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var response = await _http.PutAsJsonAsync($"api/Categories/{id}", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "Failed to update category");
            }
            return View(category);
        }

        // GET: /AdminCategory/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _http.GetFromJsonAsync<Category>($"api/Categories/{id}");
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: /AdminCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _http.DeleteAsync($"api/Categories/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to delete category");
            return RedirectToAction(nameof(Index));
        }
    }
}
