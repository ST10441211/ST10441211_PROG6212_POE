using Microsoft.AspNetCore.Mvc;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
