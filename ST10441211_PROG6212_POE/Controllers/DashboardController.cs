using Microsoft.AspNetCore.Mvc;
using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var model = new DashboardViewModel
            {
                CurrentUserName = user.FullName,
                Role = user.Role,
                // Fixed: Use LecturerId instead of Email (ClaimModel doesn't have Email property)
                MyClaims = _context.Claims.Where(c => c.LecturerId == user.Id).ToList(),
                // Fixed: Compare Role enum to Role enum, not string
                AllClaims = user.Role != Role.Lecturer ? _context.Claims.ToList() : null
            };

            return View(model);
        }
    }
}