using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StyleHub.Models;
using System.Net.Http.Json;

namespace StyleHub.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient client;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("StyleHubClient");
        }

        public async Task<IActionResult> Index(int? id, decimal? minPrice, decimal? maxPrice, string search, string priceRange)
        {
            var url = id.HasValue
                ? $"/api/Products?categoryId={id.Value}"
                : "/api/Products";

            var productsFromApi = await client.GetFromJsonAsync<List<Product>>(url);
            var categoriesFromApi = await client.GetFromJsonAsync<List<Category>>("/api/Categories");

            var products = productsFromApi?.Select(p =>
            {
                p.Category = categoriesFromApi?.FirstOrDefault(c => c.Id == p.CategoryId);
                return p;
            }).ToList();

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (minPrice.HasValue)
                products = products.Where(p => (decimal)p.Price >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                products = products.Where(p => (decimal)p.Price <= maxPrice.Value).ToList();

            if (!string.IsNullOrEmpty(priceRange))
            {
                var parts = priceRange.Split('-');
                if (parts.Length == 2 &&
                    decimal.TryParse(parts[0], out decimal rangeMin) &&
                    decimal.TryParse(parts[1], out decimal rangeMax))
                {
                    products = products.Where(p => (decimal)p.Price >= rangeMin && (decimal)p.Price <= rangeMax).ToList();
                }
            }

            ViewBag.CategoryName = categoriesFromApi?.FirstOrDefault(c => c.Id == id)?.Name;
            ViewBag.Categories = categoriesFromApi;
            ViewBag.Search = search;
            ViewBag.PriceRange = priceRange;

            return View(products);
        }

        public async Task<IActionResult> Product_detailsAsync(int id)
        {
            HttpResponseMessage response = await client.GetAsync($"/api/Products/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(json);
            var categoriesFromApi = await client.GetFromJsonAsync<List<Category>>("/api/Categories");
            ViewBag.CategoryName = categoriesFromApi?.FirstOrDefault(c => c.Id == product.CategoryId)?.Name; return View(product);
        }
    }
}