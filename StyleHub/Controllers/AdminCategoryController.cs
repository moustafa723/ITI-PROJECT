using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Headers;
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
            _http.BaseAddress = new Uri("http://stylehubteamde.runasp.net"); // 🔁 API Base URL
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _http.GetFromJsonAsync<List<Category>>("api/Categories");
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCategory vm)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(vm.Name), "Name");
            content.Add(new StringContent(vm.Back_Color ?? "#FFFFFF"), "Back_Color");

            if (vm.Photo != null)
            {
                var stream = vm.Photo.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(vm.Photo.ContentType);
                content.Add(fileContent, "Photo", vm.Photo.FileName);
            }

            var response = await _http.PostAsync("api/Categories", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to create category");
            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _http.GetFromJsonAsync<Category>($"api/Categories/{id}");
            if (category == null) return NotFound();

            var vm = new AdminCategory
            {
                Id = category.Id,
                Name = category.Name,
                Back_Color = category.Back_Color
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminCategory vm)
        {
            if (id != vm.Id) return BadRequest();

            var content = new MultipartFormDataContent();
            content.Add(new StringContent(vm.Name), "Name");
            content.Add(new StringContent(vm.Back_Color ?? "#FFFFFF"), "Back_Color");

            if (vm.Photo != null)
            {
                var stream = vm.Photo.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(vm.Photo.ContentType);
                content.Add(fileContent, "Photo", vm.Photo.FileName);
            }

            var response = await _http.PutAsync($"api/Categories/{id}", content);
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to update category");
            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _http.GetFromJsonAsync<Category>($"api/Categories/{id}");
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _http.DeleteAsync($"api/Categories/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Failed to delete category");
            return RedirectToAction(nameof(Index));
        }

     
    }
}
