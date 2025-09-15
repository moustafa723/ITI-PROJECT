using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Headers;

namespace StyleHub.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductController : Controller
    {
        private readonly HttpClient _http;

        public AdminProductController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient("StyleHubClient");
        }

        private async Task<List<Category>> GetCategoriesAsync()
        {
            return await _http.GetFromJsonAsync<List<Category>>("api/categories");
        }

        public async Task<IActionResult> Index()
        {
            var products = await _http.GetFromJsonAsync<List<Product>>("api/products");
            var categories = await GetCategoriesAsync();
            foreach (var product in products)
                product.Category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await GetCategoriesAsync();
            return View();
        }

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
                int index = 0;
                foreach (var image in vm.Images)
                {
                    var stream = image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                    content.Add(fileContent, "Images", $"img_{index++}_{image.FileName}");
                }
            }

            var response = await _http.PostAsync("api/products", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Categories = await GetCategoriesAsync();
            ModelState.AddModelError("", "Failed to create product");
            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _http.GetFromJsonAsync<Product>($"api/products/{id}");
            ViewBag.Categories = await GetCategoriesAsync();

            var vm = new ProductVmAdmin
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Size = product.Size,
                Review = product.Review,
                InStock = product.InStock,
                Badge = product.Badge,
                OldPrice = product.OldPrice,
                Rating = product.Rating,
                Color = product.Color,
                Alts = product.Alts,
                CategoryId = product.CategoryId,
                Category = product.Category
            };

            return View(vm);
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
                int index = 0;
                foreach (var image in vm.Images)
                {
                    var stream = image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
                    content.Add(fileContent, "Images", $"img_{index++}_{image.FileName}");
                }
            }

            var response = await _http.PutAsync($"api/products/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ViewBag.Categories = await GetCategoriesAsync();
            ModelState.AddModelError("", "Failed to update product");
            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _http.GetFromJsonAsync<Product>($"api/products/{id}");
            var categories = await GetCategoriesAsync();

            product.Category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _http.DeleteAsync($"api/products/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}