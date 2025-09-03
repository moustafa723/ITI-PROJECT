using Microsoft.AspNetCore.Mvc;

namespace StyleHub.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
