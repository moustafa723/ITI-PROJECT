using Microsoft.AspNetCore.Mvc;

namespace StyleHub.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }

    }
}
