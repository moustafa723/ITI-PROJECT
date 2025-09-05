using Microsoft.AspNetCore.Mvc;
using StyleHub.Models;
using System.Net.Http.Json;

namespace StyleHub.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public CategoryController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // index ممكن ياخد categoryId اختياري
        public async Task<IActionResult> Index(int? id)
        {
            var client = _clientFactory.CreateClient();

            var url = id.HasValue
                ? $"https://localhost:44374/api/Products?categoryId={id.Value}"
                : "https://localhost:44374/api/Products";

            var productsFromApi = await client.GetFromJsonAsync<List<Product>>(url);
            var categoriesFromApi = await client.GetFromJsonAsync<List<Category>>("https://localhost:44374/api/Categories");
            ViewBag.CategoryName = categoriesFromApi?.FirstOrDefault(c => c.Id == id)?.Name;
            var products = productsFromApi?.Select(p =>
            {
                p.Category = categoriesFromApi?.FirstOrDefault(c => c.Id == p.CategoryId);
                return p;
            });

            return View(products?.ToList());
        }


        public IActionResult Product_details()
        {
            return View();
        }
    }
}
