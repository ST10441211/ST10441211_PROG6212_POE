using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Models;
using System.Linq;

namespace ST10441211_PROG6212_POE.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SessionManager _session;

        public DashboardController(ApplicationDbContext context, SessionManager session)
        {
            _context = context;
            _session = session;
        }

        public IActionResult Index()
        {
            // Check login session
            if (!_session.IsLoggedIn)
            {
                TempData["ErrorMessage"] = "You must be logged in to view the dashboard.";
                return RedirectToAction("Login", "Account");
            }

            // Get current user
            var userId = _session.GetUserId();
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login", "Account");
            }

            // Build dashboard model
            var model = new DashboardViewModel
            {
                CurrentUserName = user.FullName,
                Role = user.Role,
                MyClaims = _context.Claims
                    .Where(c => c.LecturerId == user.Id)
                    .ToList(),
                AllClaims = user.Role != Role.Lecturer
                    ? _context.Claims.ToList()
                    : null
            };

            // Return the Razor view
            return View("Dashboard", model);
        }
    }
}
