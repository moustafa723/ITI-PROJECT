using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StyleHub.Models;
using System.Net.Http.Json;

namespace StyleHub.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private HttpClient client = new HttpClient();

        public CategoryController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // index ممكن ياخد categoryId اختياري
        //public async Task<IActionResult> Index(int? id)
        //{
        //    var client = _clientFactory.CreateClient();

        //    var url = id.HasValue
        //        ? $"https://localhost:44374/api/Products?categoryId={id.Value}"
        //        : "https://localhost:44374/api/Products";

        //    var productsFromApi = await client.GetFromJsonAsync<List<Product>>(url);
        //    var categoriesFromApi = await client.GetFromJsonAsync<List<Category>>("https://localhost:7158/api/Categories");
        //    ViewBag.CategoryName = categoriesFromApi?.FirstOrDefault(c => c.Id == id)?.Name;
        //    ViewBag.Categories = await client.GetFromJsonAsync<List<Category>>("https://localhost:44374/api/Categories");
        //    var products = productsFromApi?.Select(p =>
        //    {
        //        p.Category = categoriesFromApi?.FirstOrDefault(c => c.Id == p.CategoryId);
        //        return p;
        //    });

        //    return View(products?.ToList());
        //}
        public async Task<IActionResult> Index(int? id, decimal? minPrice, decimal? maxPrice, string search, string priceRange)
        {
            var client = _clientFactory.CreateClient();

            var url = id.HasValue
                ? $"https://localhost:7158/api/Products?categoryId={id.Value}"
                : "https://localhost:7158/api/Products";

            var productsFromApi = await client.GetFromJsonAsync<List<Product>>(url);
            var categoriesFromApi = await client.GetFromJsonAsync<List<Category>>("https://localhost:7158/api/Categories");

            var products = productsFromApi?.Select(p =>
            {
                p.Category = categoriesFromApi?.FirstOrDefault(c => c.Id == p.CategoryId);
                return p;
            }).ToList();

            // فلترة حسب البحث
            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            // فلترة حسب السعر من min/max
            if (minPrice.HasValue)
                products = products.Where(p => (decimal)p.Price >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                products = products.Where(p => (decimal)p.Price <= maxPrice.Value).ToList();

            // فلترة حسب dropdown PriceRange
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
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7158/api/Products/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var json = await response.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<Product>(json);

            return View(product);
        }
    }
}
