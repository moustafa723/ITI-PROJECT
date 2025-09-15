using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StyleHub.Models;
using StyleHub.Models.HomeViewModel;
using System.Diagnostics;
using System.Text.Json;

namespace StyleHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory
        )
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("StyleHubClient");
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel();

            var categoriesResponse = await _httpClient.GetAsync("/api/categories");
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var json = await categoriesResponse.Content.ReadAsStringAsync();
                vm.Categories = JsonConvert.DeserializeObject<List<Category>>(json);
            }

            var bestSellersResponse = await _httpClient.GetAsync("api/products?filter=featured");
            if (bestSellersResponse.IsSuccessStatusCode)
            {
                var json = await bestSellersResponse.Content.ReadAsStringAsync();
                var all = JsonConvert.DeserializeObject<List<Product>>(json);
                vm.BestSellers = all.Take(4).ToList();
            }

            return View(vm);
        }

        public async Task<List<Product>> GetTopBestSellersAsync(int count = 4)
        {
            var response = await _httpClient.GetAsync("api/products?filter=featured");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var all = JsonConvert.DeserializeObject<List<Product>>(json);
            return all.Take(count).ToList();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}