using Microsoft.AspNetCore.Mvc;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
