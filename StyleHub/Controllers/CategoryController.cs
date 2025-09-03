using Microsoft.AspNetCore.Mvc;

namespace StyleHub.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Product_details() 
        {
            return View();
        }
    }
}
