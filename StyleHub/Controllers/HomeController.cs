using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StyleHub.Models;
using System.Diagnostics;
using System.Text.Json;

namespace StyleHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        public HomeController(ILogger<HomeController> logger , IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7158/api/");

        }
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("categories");
            if (!response.IsSuccessStatusCode) return View(new List<Category>());

            var json = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<Category>>(json);

            return View(categories);
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
