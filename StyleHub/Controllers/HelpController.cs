using Microsoft.AspNetCore.Mvc;

namespace StyleHub.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Support()
        {
            return View();
        }

        public IActionResult Shiping_info()
        {
            return View();
        }

        public IActionResult Return_policy()
        {
            return View();
        }

        public IActionResult Tos()
        {
            return View();
        }
    }
}