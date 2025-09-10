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
        private async Task<List<Category>> GetCategoriesAsync()
        {
            return await _http.GetFromJsonAsync<List<Category>>("api/Categories");
        }


        // GET: /AdminProduct
        public async Task<IActionResult> Index()
        {
            var products = await _http.GetFromJsonAsync<List<Product>>("api/Products");
            var categories = await _http.GetFromJsonAsync<List<Category>>("api/Categories");

            foreach (var p in products)
            {
                p.Category = categories.FirstOrDefault(c => c.Id == p.CategoryId);
            }


            return View(products);
        }

        // GET: /AdminProduct/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await GetCategoriesAsync();
            return View();
        }

        // POST: /AdminProduct/Create
        // POST: /AdminProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVmAdmin vm)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(vm.Name), "Name");
            content.Add(new StringContent(vm.Price.ToString()), "Price");
            content.Add(new StringContent(vm.Size ?? ""), "Size");
            content.Add(new StringContent(vm.Review.ToString()), "Review");
            content.Add(new StringContent(vm.InStock.ToString()), "InStock");
            content.Add(new StringContent(vm.Badge ?? ""), "Badge");
            content.Add(new StringContent(vm.OldPrice?.ToString() ?? ""), "OldPrice");
            content.Add(new StringContent(vm.Rating.ToString()), "Rating");
            content.Add(new StringContent(vm.Color ?? ""), "Color");
            content.Add(new StringContent(vm.Alts ?? ""), "Alts");
            content.Add(new StringContent(vm.CategoryId.ToString()), "CategoryId");

            if (vm.Images != null)
            {
                foreach (var image in vm.Images)
                {
                    var stream = image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                    content.Add(fileContent, "Images", image.FileName);
                }
            }

            var response = await _http.PostAsync("api/products", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Categories = await GetCategoriesAsync();
            ModelState.AddModelError("", "Failed to create product");
            return View(vm);
        }

        // GET: /AdminProduct/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {            var product = await _http.GetFromJsonAsync<Product>($"api/Products/{id}");

            ViewBag.Categories = await GetCategoriesAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVmAdmin vm)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(vm.Name), "Name");
            content.Add(new StringContent(vm.Price.ToString()), "Price");
            content.Add(new StringContent(vm.Size ?? ""), "Size");
            content.Add(new StringContent(vm.Review.ToString()), "Review");
            content.Add(new StringContent(vm.InStock.ToString()), "InStock");
            content.Add(new StringContent(vm.Badge ?? ""), "Badge");
            content.Add(new StringContent(vm.OldPrice?.ToString() ?? ""), "OldPrice");
            content.Add(new StringContent(vm.Rating.ToString()), "Rating");
            content.Add(new StringContent(vm.Color ?? ""), "Color");
            content.Add(new StringContent(vm.Alts ?? ""), "Alts");
            content.Add(new StringContent(vm.CategoryId.ToString()), "CategoryId");

            if (vm.Images != null)
            {
                foreach (var image in vm.Images)
                {
                    var stream = image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(image.ContentType);
                    content.Add(fileContent, "Images", image.FileName);
                }
            }

            var response = await _http.PutAsync($"api/products/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Categories = await GetCategoriesAsync();
            ModelState.AddModelError("", "Failed to update product");
            return View(vm);
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
